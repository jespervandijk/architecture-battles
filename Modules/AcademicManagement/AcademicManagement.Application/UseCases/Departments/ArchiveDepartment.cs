using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace AcademicManagement.Application.UseCases.Departments;

public class ArchiveDepartmentEndpoint : Endpoint<ArchiveDepartment, DepartmentId>
{
    public override void Configure()
    {
        Post("academic-management/departments/archive");
        Policies(PolicyAcademicManagement.PresidentOnly);
        Description(x => x.WithTags("academic-management/departments"));
    }

    public override async Task HandleAsync(ArchiveDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record ArchiveDepartment : ICommand<DepartmentId>
{
    public required DepartmentId DepartmentId { get; init; }
}

public class ArchiveDepartmentHandler : ICommandHandler<ArchiveDepartment, DepartmentId>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public ArchiveDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IUserContextService userContextService)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _userContextService = userContextService;
    }

    public async Task<DepartmentId> ExecuteAsync(ArchiveDepartment command, CancellationToken ct)
    {
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
        var university = await _universityRepository.GetByIdAsync(department.UniversityId);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You must be the president of the university that owns this department");
        }

        department.Archive();
        _departmentRepository.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return department.Id;
    }
}

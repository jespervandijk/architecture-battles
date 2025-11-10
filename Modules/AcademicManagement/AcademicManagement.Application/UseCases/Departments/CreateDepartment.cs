using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;

namespace AcademicManagement.Application.UseCases.Departments;

public class CreateDepartmentEndpoint : Endpoint<CreateDepartment, DepartmentId>
{
    public override void Configure()
    {
        Post("academic-management/department/create");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(CreateDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateDepartment : ICommand<DepartmentId>
{
    public required UniversityId UniversityId { get; init; }
    public required Name Name { get; init; }
    public required ProfessorId HeadOfDepartment { get; init; }
}

public class CreateDepartmentHandler : ICommandHandler<CreateDepartment, DepartmentId>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IUserContextService userContextService)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _userContextService = userContextService;
    }

    public async Task<DepartmentId> ExecuteAsync(CreateDepartment command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.UniversityId);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You must be the president of this university");
        }

        var department = Department.Create(command.UniversityId, command.Name, command.HeadOfDepartment);
        _departmentRepository.Insert(department);
        await _unitOfWork.SaveChangesAsync();
        return department.Id;
    }
}

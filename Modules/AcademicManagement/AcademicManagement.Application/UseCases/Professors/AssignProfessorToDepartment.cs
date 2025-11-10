using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Professors;

public class AssignProfessorToDepartmentEndpoint : Endpoint<AssignProfessorToDepartment, ProfessorId>
{
    public override void Configure()
    {
        Post("academic-management/professor/assign-to-department");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(AssignProfessorToDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record AssignProfessorToDepartment : ICommand<ProfessorId>
{
    public required ProfessorId ProfessorId { get; init; }
    public required DepartmentId DepartmentId { get; init; }
    public bool AsHeadOfDepartment { get; init; }
}

public class AssignProfessorToDepartmentHandler : ICommandHandler<AssignProfessorToDepartment, ProfessorId>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public AssignProfessorToDepartmentHandler(IProfessorRepository professorRepository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IUserContextService userContextService)
    {
        _professorRepository = professorRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _userContextService = userContextService;
    }

    public async Task<ProfessorId> ExecuteAsync(AssignProfessorToDepartment command, CancellationToken ct)
    {
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId);
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId);
        var university = await _universityRepository.GetByIdAsync(professor.WorkPlace);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You are not authorized to manage this professor");
        }

        if (professor.WorkPlace != department.UniversityId)
        {
            throw new InvalidOperationException("Professor can only be assigned to a department in their workplace university");
        }

        professor.AssignToDepartment(command.DepartmentId);
        _professorRepository.Update(professor);

        if (command.AsHeadOfDepartment)
        {
            department.HeadOfDepartment = command.ProfessorId;
            _departmentRepository.Update(department);
        }

        await _unitOfWork.SaveChangesAsync();
        return professor.Id;
    }
}

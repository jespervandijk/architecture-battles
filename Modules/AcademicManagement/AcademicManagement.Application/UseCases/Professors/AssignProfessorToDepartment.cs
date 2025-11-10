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
    private readonly IUnitOfWork _unitOfWork;

    public AssignProfessorToDepartmentHandler(IProfessorRepository professorRepository, IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfessorId> ExecuteAsync(AssignProfessorToDepartment command, CancellationToken ct)
    {
        var professor = await _professorRepository.GetByIdAsync(command.ProfessorId) ?? throw new InvalidOperationException($"Professor with id {command.ProfessorId} was not found.");
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId) ?? throw new InvalidOperationException($"Department with id {command.DepartmentId} was not found.");

        if (professor.WorkPlace != department.UniversityId)
        {
            throw new InvalidOperationException("Professor can only be assigned to a department in their workplace university.");
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

public class AssignProfessorToDepartmentValidator : Validator<AssignProfessorToDepartment>
{
    public AssignProfessorToDepartmentValidator()
    {
        _ = RuleFor(x => x.ProfessorId).NotEmpty();
        _ = RuleFor(x => x.DepartmentId).NotEmpty();

        _ = RuleFor(x => x.ProfessorId)
            .MustAsync(async (professorId, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var professor = await professorRepo.GetByIdAsync(professorId);
                return professor is not null;
            })
            .WithMessage("Professor not found");

        _ = RuleFor(x => x.DepartmentId)
            .MustAsync(async (departmentId, ct) =>
            {
                var departmentRepo = Resolve<IDepartmentRepository>();
                var department = await departmentRepo.GetByIdAsync(departmentId);
                return department is not null;
            })
            .WithMessage("Department not found");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var professor = await professorRepo.GetByIdAsync(request.ProfessorId);
                var university = await universityRepo.GetByIdAsync(professor.WorkPlace);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .WithMessage("You are not authorized to manage this professor");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var professorRepo = Resolve<IProfessorRepository>();
                var departmentRepo = Resolve<IDepartmentRepository>();

                var professor = await professorRepo.GetByIdAsync(request.ProfessorId);
                var department = await departmentRepo.GetByIdAsync(request.DepartmentId);

                return professor.WorkPlace == department.UniversityId;
            })
            .WithMessage("Professor can only be assigned to a department in their workplace university");
    }
}

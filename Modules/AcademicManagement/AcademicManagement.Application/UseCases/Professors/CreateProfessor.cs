using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using FastEndpoints;
using FluentValidation;
using Qowaiv;

namespace AcademicManagement.Application.UseCases.Professors;

public class CreateProfessorEndpoint : Endpoint<CreateProfessor, ProfessorId>
{
    public override void Configure()
    {
        Post("academic-management/professor/create");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(CreateProfessor req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record CreateProfessor : ICommand<ProfessorId>
{
    public required string UserName { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required EmailAddress EmailAddress { get; init; }
    public required Rank Rank { get; init; }
    public required UniversityId WorkPlace { get; init; }
    public DepartmentId? DepartmentId { get; init; }
}

public class CreateProfessorHandler : ICommandHandler<CreateProfessor, ProfessorId>
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateProfessorHandler(IProfessorRepository professorRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _professorRepository = professorRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ProfessorId> ExecuteAsync(CreateProfessor command, CancellationToken ct)
    {
        var userName = UserName.From(command.UserName);
        var user = User.Create(userName, UserRole.Professor);
        _userRepository.Insert(user);

        var professor = Professor.Create(
            command.FirstName,
            command.LastName,
            command.EmailAddress,
            command.Rank,
            command.WorkPlace,
            user.Id,
            command.DepartmentId
        );
        _professorRepository.Insert(professor);
        await _unitOfWork.SaveChangesAsync();
        return professor.Id;
    }
}

public class CreateProfessorValidator : Validator<CreateProfessor>
{
    public CreateProfessorValidator()
    {
        _ = RuleFor(x => x.UserName).NotEmpty();
        _ = RuleFor(x => x.FirstName).NotEmpty();
        _ = RuleFor(x => x.LastName).NotEmpty();
        _ = RuleFor(x => x.EmailAddress).NotEmpty();
        _ = RuleFor(x => x.Rank).IsInEnum();
        _ = RuleFor(x => x.WorkPlace).NotEmpty();

        _ = RuleFor(x => x.WorkPlace)
            .MustAsync(async (workPlace, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var university = await universityRepo.GetByIdAsync(workPlace);
                return university is not null;
            })
            .WithMessage("University not found")
            .MustAsync(async (workPlace, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var university = await universityRepo.GetByIdAsync(workPlace);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .WithMessage("You are not authorized to create professors for this university");

        _ = RuleFor(x => x.DepartmentId)
            .MustAsync(async (departmentId, ct) =>
            {
                var departmentRepo = Resolve<IDepartmentRepository>();
                var department = await departmentRepo.GetByIdAsync(departmentId!.Value);
                return department is not null;
            })
            .When(x => x.DepartmentId is not null)
            .WithMessage("Department not found");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                if (request.DepartmentId is null)
                {
                    return true;
                }

                var departmentRepo = Resolve<IDepartmentRepository>();
                var department = await departmentRepo.GetByIdAsync(request.DepartmentId.Value);

                return department.UniversityId == request.WorkPlace;
            })
            .When(x => x.DepartmentId is not null)
            .WithMessage("Department must belong to the same university as the professor's workplace");
    }
}

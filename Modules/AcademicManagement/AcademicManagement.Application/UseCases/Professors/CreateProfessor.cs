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
    private readonly IUniversityRepository _universityRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContextService _userContextService;

    public CreateProfessorHandler(IProfessorRepository professorRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, IUniversityRepository universityRepository, IDepartmentRepository departmentRepository, IUserContextService userContextService)
    {
        _professorRepository = professorRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _universityRepository = universityRepository;
        _departmentRepository = departmentRepository;
        _userContextService = userContextService;
    }

    public async Task<ProfessorId> ExecuteAsync(CreateProfessor command, CancellationToken ct)
    {
        var university = await _universityRepository.GetByIdAsync(command.WorkPlace);

        var presidentId = _userContextService.GetPresidentId();
        if (university.President != presidentId)
        {
            throw new UnauthorizedAccessException("You are not authorized to create professors for this university");
        }

        if (command.DepartmentId is not null)
        {
            var department = await _departmentRepository.GetByIdAsync(command.DepartmentId.Value);
            if (department.UniversityId != command.WorkPlace)
            {
                throw new InvalidOperationException("Department must belong to the same university as the professor's workplace");
            }
        }

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

using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

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
    private readonly IUnitOfWork _unitOfWork;

    public CreateDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentId> ExecuteAsync(CreateDepartment command, CancellationToken ct)
    {
        var department = Department.Create(command.UniversityId, command.Name, command.HeadOfDepartment);
        _departmentRepository.Insert(department);
        await _unitOfWork.SaveChangesAsync();
        return department.Id;
    }
}

public class CreateDepartmentValidator : Validator<CreateDepartment>
{
    public CreateDepartmentValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.HeadOfDepartment).NotEmpty();
        _ = RuleFor(x => x.UniversityId)
            .NotEmpty()
            .MustAsync(async (universityId, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var university = await universityRepo.GetByIdAsync(universityId);
                return university is not null;
            })
            .WithMessage("University not found")
            .MustAsync(async (universityId, ct) =>
            {
                return await AuthorizationRules.UserIsPresidentOfUniversity(
                    Resolve<IUserContextService>(),
                    Resolve<IUniversityRepository>(),
                    universityId);
            })
            .WithMessage("You must be the president of this university");
    }
}

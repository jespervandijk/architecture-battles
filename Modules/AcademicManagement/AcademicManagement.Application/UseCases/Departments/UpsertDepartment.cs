using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Departments;

public class UpsertDepartmentEndpoint : Endpoint<UpsertDepartment, DepartmentId>
{
    public override void Configure()
    {
        Post("academic-management/department/upsert");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(UpsertDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpsertDepartment : ICommand<DepartmentId>
{
    public DepartmentId? ExistingDepartmentId { get; init; }
    public UniversityId? UniversityId { get; init; }
    public required Name Name { get; init; }
    public required ProfessorId HeadOfDepartment { get; init; }
}

public class UpsertDepartmentHandler : ICommandHandler<UpsertDepartment, DepartmentId>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentId> ExecuteAsync(UpsertDepartment command, CancellationToken ct)
    {
        if (command.ExistingDepartmentId is not null)
        {
            var existingDepartment = await _departmentRepository.GetByIdAsync(command.ExistingDepartmentId.Value) ?? throw new InvalidOperationException($"Department with id {command.ExistingDepartmentId} was not found.");
            existingDepartment.Update(command.Name, command.HeadOfDepartment);
            _departmentRepository.Update(existingDepartment);
            await _unitOfWork.SaveChangesAsync();
            return existingDepartment.Id;
        }

        if (command.UniversityId is null)
        {
            throw new InvalidOperationException("UniversityId is required when creating a new department.");
        }

        var department = Department.Create(command.UniversityId.Value, command.Name, command.HeadOfDepartment);
        _departmentRepository.Insert(department);
        await _unitOfWork.SaveChangesAsync();
        return department.Id;
    }
}

public class UpsertDepartmentValidator : Validator<UpsertDepartment>
{
    public UpsertDepartmentValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.HeadOfDepartment).NotEmpty();

        _ = RuleFor(x => x.UniversityId)
            .Null()
            .When(x => x.ExistingDepartmentId is not null)
            .WithMessage("UniversityId must be null when updating an existing department");

        _ = RuleFor(x => x.UniversityId)
            .NotEmpty()
            .When(x => x.ExistingDepartmentId is null)
            .WithMessage("UniversityId is required when creating a new department");

        _ = RuleFor(x => x.UniversityId)
            .MustAsync(async (universityId, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var university = await universityRepo.GetByIdAsync(universityId!.Value);
                return university is not null;
            })
            .When(x => x.UniversityId is not null)
            .WithMessage("University not found")
            .MustAsync(async (universityId, ct) =>
            {
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var university = await universityRepo.GetByIdAsync(universityId!.Value);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .When(x => x.UniversityId is not null)
            .WithMessage("You are not authorized to manage departments for this university");

        _ = RuleFor(x => x)
            .MustAsync(async (request, ct) =>
            {
                var departmentRepo = Resolve<IDepartmentRepository>();
                var department = await departmentRepo.GetByIdAsync(request.ExistingDepartmentId!.Value);

                if (department is null)
                {
                    return false;
                }

                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var university = await universityRepo.GetByIdAsync(department.UniversityId);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .When(x => x.ExistingDepartmentId is not null)
            .WithMessage("You are not authorized to modify this department");
    }
}

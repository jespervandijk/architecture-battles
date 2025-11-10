using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
using AcademicManagement.Domain.Aggregates.Departments;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Departments;

public class ArchiveDepartmentEndpoint : Endpoint<ArchiveDepartment, DepartmentId>
{
    public override void Configure()
    {
        Post("academic-management/department/archive");
        Policies(PolicyAcademicManagement.PresidentOnly);
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
    private readonly IUnitOfWork _unitOfWork;

    public ArchiveDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentId> ExecuteAsync(ArchiveDepartment command, CancellationToken ct)
    {
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId) ?? throw new InvalidOperationException($"Department with id {command.DepartmentId} not found.");
        department.IsArchived = true;
        _departmentRepository.Update(department);
        await _unitOfWork.SaveChangesAsync();

        return department.Id;
    }
}

public class ArchiveDepartmentValidator : Validator<ArchiveDepartment>
{
    public ArchiveDepartmentValidator()
    {
        _ = RuleFor(x => x.DepartmentId)
            .NotEmpty()
            .MustAsync(async (departmentId, ct) =>
            {
                var departmentRepo = Resolve<IDepartmentRepository>();
                var department = await departmentRepo.GetByIdAsync(departmentId);
                return department is not null;
            })
            .WithMessage("Department not found")
            .MustAsync(async (departmentId, ct) =>
            {
                return await AuthorizationRules.UserIsPresidentOfDepartmentUniversity(
                    Resolve<IUserContextService>(),
                    Resolve<IDepartmentRepository>(),
                    Resolve<IUniversityRepository>(),
                    departmentId);
            })
            .WithMessage("You must be the president of the university that owns this department");
    }
}

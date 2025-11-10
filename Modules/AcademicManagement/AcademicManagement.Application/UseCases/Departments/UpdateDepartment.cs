using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Validation;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;

namespace AcademicManagement.Application.UseCases.Departments;

public class UpdateDepartmentEndpoint : Endpoint<UpdateDepartment, DepartmentId>
{
    public override void Configure()
    {
        Post("academic-management/department/update");
        Policies(PolicyAcademicManagement.PresidentOnly);
    }

    public override async Task HandleAsync(UpdateDepartment req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpdateDepartment : ICommand<DepartmentId>
{
    public required DepartmentId DepartmentId { get; init; }
    public required Name Name { get; init; }
    public required ProfessorId HeadOfDepartment { get; init; }
}

public class UpdateDepartmentHandler : ICommandHandler<UpdateDepartment, DepartmentId>
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDepartmentHandler(IDepartmentRepository departmentRepository, IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<DepartmentId> ExecuteAsync(UpdateDepartment command, CancellationToken ct)
    {
        var department = await _departmentRepository.GetByIdAsync(command.DepartmentId) ?? throw new InvalidOperationException($"Department with id {command.DepartmentId} was not found.");
        department.Update(command.Name, command.HeadOfDepartment);
        _departmentRepository.Update(department);
        await _unitOfWork.SaveChangesAsync();
        return department.Id;
    }
}

public class UpdateDepartmentValidator : Validator<UpdateDepartment>
{
    public UpdateDepartmentValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.HeadOfDepartment).NotEmpty();
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

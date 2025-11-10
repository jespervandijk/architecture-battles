using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
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
                var departmentRepo = Resolve<IDepartmentRepository>();
                var universityRepo = Resolve<IUniversityRepository>();
                var userContext = Resolve<IUserContextService>();

                var department = await departmentRepo.GetByIdAsync(departmentId);
                var university = await universityRepo.GetByIdAsync(department.UniversityId);
                var currentUser = userContext.GetCurrentUser();

                var presidentId = PresidentId.From(currentUser.Id.Value);
                return university.President == presidentId;
            })
            .WithMessage("You are not authorized to modify this department");
    }
}

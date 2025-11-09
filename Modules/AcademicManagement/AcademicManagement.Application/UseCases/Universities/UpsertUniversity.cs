using AcademicManagement.Application.Abstractions;
using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;
using FastEndpoints;
using FluentValidation;
using PresidentId = AcademicManagement.Domain.Aggregates.Presidents.PresidentId;
using UniversityId = AcademicManagement.Domain.Aggregates.Universities.UniversityId;

namespace AcademicManagement.Application.UseCases.Universities;

public class UpsertUniversityEndpoint : Endpoint<UpsertUniversity, UniversityId>
{
    public override void Configure()
    {
        Post("academic-management/university/upsert");
        AllowAnonymous();
    }

    public override async Task HandleAsync(UpsertUniversity req, CancellationToken ct)
    {
        Response = await req.ExecuteAsync(ct);
    }
}

public record UpsertUniversity : ICommand<UniversityId>
{
    public UniversityId? ExistingUniversityId { get; init; }
    public required Name Name { get; init; }
    public required PresidentId President { get; init; }
}

public class UpsertUniversityHandler : ICommandHandler<UpsertUniversity, UniversityId>
{
    private readonly IUniversityRepository _universityRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpsertUniversityHandler(IUniversityRepository universityRepository, IUnitOfWork unitOfWork)
    {
        _universityRepository = universityRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UniversityId> ExecuteAsync(UpsertUniversity command, CancellationToken ct)
    {
        if (command.ExistingUniversityId is not null)
        {
            var existingUniversity = await _universityRepository.GetByIdAsync(command.ExistingUniversityId.Value);
            if (existingUniversity is null)
            {
                throw new InvalidOperationException($"University with id {command.ExistingUniversityId} was not found.");
            }
            existingUniversity.Name = command.Name;
            existingUniversity.President = command.President;
            _universityRepository.Update(existingUniversity);
            await _unitOfWork.SaveChangesAsync();
            return existingUniversity.Id;
        }

        var university = University.Create(command.President, command.Name);
        _universityRepository.Insert(university);
        await _unitOfWork.SaveChangesAsync();
        return university.Id;
    }
}

public class UpsertUniversityValidator : Validator<UpsertUniversity>
{
    public UpsertUniversityValidator()
    {
        _ = RuleFor(x => x.Name).NotEmpty();
        _ = RuleFor(x => x.President).NotEmpty();
    }
}
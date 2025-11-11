using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Universities;

public sealed class University
{
    public UniversityId Id { get; init; }
    public PresidentId President { get; init; }
    public Name Name { get; private set; }
    private readonly List<ProfessorId> _professors;
    public IReadOnlyList<ProfessorId> Professors => _professors;
    public bool IsArchived { get; private set; }

    [JsonConstructor]
    private University(UniversityId id, PresidentId president, Name name, List<ProfessorId> professors, bool isArchived)
    {
        Id = id;
        President = president;
        Name = name;
        IsArchived = isArchived;
        _professors = professors;
    }

    public static University Create(PresidentId president, Name name)
    {
        return new University(UniversityId.Next(), president, name, [], false);
    }

    public void Update(Name name)
    {
        Name = name;
    }

    public void Archive()
    {
        IsArchived = true;
    }
}

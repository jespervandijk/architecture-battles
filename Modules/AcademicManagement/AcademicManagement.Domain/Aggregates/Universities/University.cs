using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Universities;

public sealed class University
{
    public UniversityId Id { get; init; }

    public PresidentId President { get; set; }

    public Name Name { get; set; }

    public List<ProfessorId> Professors { get; set; }

    public bool IsArchived { get; set; }

    [JsonConstructor]
    private University(UniversityId id, PresidentId president, Name name, List<ProfessorId> professors, bool isArchived)
    {
        Id = id;
        President = president;
        Name = name;
        IsArchived = isArchived;
        Professors = professors;
    }

    public static University Create(PresidentId president, Name name)
    {
        return new University(UniversityId.Next(), president, name, [], false);
    }
}

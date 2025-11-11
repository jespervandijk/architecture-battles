using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Courses;

public sealed class Section
{
    public SectionId Id { get; init; }
    public string Name { get; internal set; }
    public ProfessorId Professor { get; internal set; }
    public Url? TeachingMaterials { get; internal set; }

    [JsonConstructor]
    private Section(SectionId id, string name, ProfessorId professor, Url? teachingMaterials)
    {
        Id = id;
        Name = name;
        Professor = professor;
        TeachingMaterials = teachingMaterials;
    }

    internal static Section Create(string name, ProfessorId professor)
    {
        return new Section(
            SectionId.Next(),
            name,
            professor,
            null);
    }
    internal void UpdateDetails(string name, Url? teachingMaterials)
    {
        Name = name;
        TeachingMaterials = teachingMaterials;
    }
}

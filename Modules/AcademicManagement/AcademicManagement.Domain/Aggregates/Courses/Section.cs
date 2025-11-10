using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Aggregates.Courses;

public sealed class Section
{
    public SectionId Id { get; init; }
    public string Name { get; internal set; }
    public ProfessorId Professor { get; internal set; }
    public Uri? TeachingMaterialsUrl { get; internal set; }

    [JsonConstructor]
    private Section(SectionId id, string name, ProfessorId professor, Uri? teachingMaterialsUrl)
    {
        Id = id;
        Name = name;
        Professor = professor;
        TeachingMaterialsUrl = teachingMaterialsUrl;
    }

    internal static Section Create(string name, ProfessorId professor)
    {
        return new Section(
            SectionId.Next(),
            name,
            professor,
            null);
    }

    internal void Update(string name, ProfessorId professor)
    {
        Name = name;
        Professor = professor;
    }

    internal void UpdateDetails(string name, Uri? teachingMaterialsUrl)
    {
        Name = name;
        TeachingMaterialsUrl = teachingMaterialsUrl;
    }
}

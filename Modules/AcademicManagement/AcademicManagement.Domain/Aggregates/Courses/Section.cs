using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Assignments;
using AcademicManagement.Domain.Aggregates.Exams;
using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Aggregates.Courses;

public sealed class Section
{
    public SectionId Id { get; init; }
    public string Name { get; internal set; }
    public ProfessorId Professor { get; internal set; }
    public List<AssignmentId> AssignmentIds { get; internal set; }
    public List<ExamId> ExamIds { get; internal set; }
    public Uri? TeachingMaterialsUrl { get; internal set; }

    [JsonConstructor]
    private Section(SectionId id, string name, ProfessorId professor, List<AssignmentId> assignmentIds, List<ExamId> examIds, Uri? teachingMaterialsUrl)
    {
        Id = id;
        Name = name;
        Professor = professor;
        AssignmentIds = assignmentIds;
        ExamIds = examIds;
        TeachingMaterialsUrl = teachingMaterialsUrl;
    }

    public static Section Create(string name, ProfessorId professor, Uri? teachingMaterialsUrl = null)
    {
        return new Section(
            SectionId.Next(),
            name,
            professor,
            [],
            [],
            teachingMaterialsUrl);
    }
}

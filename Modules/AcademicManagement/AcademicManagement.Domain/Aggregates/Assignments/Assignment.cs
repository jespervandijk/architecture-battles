using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Assignments;

public sealed class Assignment
{
    public AssignmentId Id { get; init; }
    public SectionId SectionId { get; init; }
    public Name Title { get; set; }
    public GradeWeight GradeWeight { get; set; }
    public Url DocumentUrl { get; set; }
    public Year SchoolYear { get; set; }
    public List<AssignmentResult> AssignmentResults { get; set; }

    [JsonConstructor]
    private Assignment(AssignmentId id, SectionId sectionId, Name title, GradeWeight gradeWeight, Year schoolYear, Url documentUrl, List<AssignmentResult> assignmentResults)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        GradeWeight = gradeWeight;
        SchoolYear = schoolYear;
        DocumentUrl = documentUrl;
        AssignmentResults = assignmentResults;
    }

    public static Assignment Create(SectionId sectionId, Name title, GradeWeight gradeWeight, Year schoolYear, Url documentUrl)
    {
        return new Assignment(
            AssignmentId.Next(),
            sectionId,
            title,
            gradeWeight,
            schoolYear,
            documentUrl,
            []);
    }
}

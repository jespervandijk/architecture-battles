using System.Text.Json.Serialization;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Assignments;

public sealed class Assignment
{
    public AssignmentId Id { get; init; }
    public Name Title { get; set; }
    public GradeWeight GradeWeight { get; set; }
    public Url DocumentUrl { get; set; }
    public Year SchoolYear { get; set; }
    public List<AssignmentResult> AssignmentResults { get; set; }

    [JsonConstructor]
    private Assignment(AssignmentId id, Name title, GradeWeight gradeWeight, Year schoolYear, Url documentUrl, List<AssignmentResult> assignmentResults)
    {
        Id = id;
        Title = title;
        GradeWeight = gradeWeight;
        SchoolYear = schoolYear;
        DocumentUrl = documentUrl;
        AssignmentResults = assignmentResults;
    }

    public static Assignment Create(Name title, GradeWeight gradeWeight, Year schoolYear, Url documentUrl)
    {
        return new Assignment(
            AssignmentId.Next(),
            title,
            gradeWeight,
            schoolYear,
            documentUrl,
            []);
    }
}

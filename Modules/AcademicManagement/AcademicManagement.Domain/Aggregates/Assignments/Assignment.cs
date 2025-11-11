using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Assignments;

public sealed class Assignment
{
    public AssignmentId Id { get; init; }
    public SectionId SectionId { get; init; }
    public Name Title { get; private set; }
    public GradeWeight GradeWeight { get; private set; }
    public Url DocumentUrl { get; private set; }
    public Year SchoolYear { get; private set; }
    private readonly List<AssignmentResult> _assignmentResults;
    public IReadOnlyList<AssignmentResult> AssignmentResults => _assignmentResults;

    [JsonConstructor]
    private Assignment(AssignmentId id, SectionId sectionId, Name title, GradeWeight gradeWeight, Year schoolYear, Url documentUrl, List<AssignmentResult> assignmentResults)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        GradeWeight = gradeWeight;
        SchoolYear = schoolYear;
        DocumentUrl = documentUrl;
        _assignmentResults = assignmentResults;
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

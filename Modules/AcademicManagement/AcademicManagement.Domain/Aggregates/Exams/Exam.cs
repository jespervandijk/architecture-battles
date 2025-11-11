using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Exams;

public sealed class Exam
{
    public ExamId Id { get; init; }
    public SectionId SectionId { get; init; }
    public Name Title { get; private set; }
    public TimeSpan Duration { get; private set; }
    public GradeWeight GradeWeight { get; private set; }
    public Year SchoolYear { get; private set; }
    public Url DocumentUrl { get; private set; }
    private readonly List<ExamResult> _examResults;
    public IReadOnlyList<ExamResult> ExamResults => _examResults;

    [JsonConstructor]
    private Exam(ExamId id, SectionId sectionId, Name title, TimeSpan duration, GradeWeight gradeWeight, Year schoolYear, Url documentUrl, List<ExamResult> examResults)
    {
        Id = id;
        SectionId = sectionId;
        Title = title;
        Duration = duration;
        GradeWeight = gradeWeight;
        SchoolYear = schoolYear;
        DocumentUrl = documentUrl;
        _examResults = examResults;
    }

    public static Exam Create(SectionId sectionId, Name title, TimeSpan duration, GradeWeight gradeWeight, Year schoolYear, Url documentUrl)
    {
        return new Exam(
            ExamId.Next(),
            sectionId,
            title,
            duration,
            gradeWeight,
            schoolYear,
            documentUrl,
            []);
    }
}

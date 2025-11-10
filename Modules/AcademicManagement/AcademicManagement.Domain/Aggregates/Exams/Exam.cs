using System.Text.Json.Serialization;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Exams;

public sealed class Exam
{
    public ExamId Id { get; init; }
    public Name Title { get; set; }
    public TimeSpan Duration { get; set; }
    public GradeWeight GradeWeight { get; set; }
    public Year SchoolYear { get; set; }
    public Url DocumentUrl { get; set; }
    public List<ExamResult> ExamResults { get; set; }

    [JsonConstructor]
    private Exam(ExamId id, Name title, TimeSpan duration, GradeWeight gradeWeight, Year schoolYear, Url documentUrl, List<ExamResult> examResults)
    {
        Id = id;
        Title = title;
        Duration = duration;
        GradeWeight = gradeWeight;
        SchoolYear = schoolYear;
        DocumentUrl = documentUrl;
        ExamResults = examResults;
    }

    public static Exam Create(Name title, TimeSpan duration, GradeWeight gradeWeight, Year schoolYear, Url documentUrl)
    {
        return new Exam(
            ExamId.Next(),
            title,
            duration,
            gradeWeight,
            schoolYear,
            documentUrl,
            []);
    }
}

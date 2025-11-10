using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Exams;

public sealed class Exam
{
    public ExamId Id { get; init; }
    public SectionId SectionId { get; init; }
    public Name Title { get; set; }
    public TimeSpan Duration { get; set; }
    public GradeWeight GradeWeight { get; set; }
    public Year SchoolYear { get; set; }
    public Url DocumentUrl { get; set; }
    public List<ExamResult> ExamResults { get; set; }

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
        ExamResults = examResults;
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

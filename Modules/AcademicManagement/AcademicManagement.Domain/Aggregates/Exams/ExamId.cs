using Vogen;

namespace AcademicManagement.Domain.Aggregates.Exams;

[ValueObject<Guid>]
public readonly partial struct ExamId
{
    public static ExamId Next() => From(Guid.NewGuid());
}

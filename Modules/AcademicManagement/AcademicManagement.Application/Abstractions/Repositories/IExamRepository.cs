using AcademicManagement.Domain.Aggregates.Exams;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IExamRepository : IRepository<Exam, ExamId>
{
}

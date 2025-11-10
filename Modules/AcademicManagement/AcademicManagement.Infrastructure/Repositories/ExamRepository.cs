using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Exams;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public sealed class ExamRepository : Repository<Exam, ExamId>, IExamRepository
{
    public ExamRepository(IDocumentSession session) : base(session)
    {
    }
}

using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using Marten;

namespace AcademicManagement.Infrastructure.Repositories;

public class CourseRepository : Repository<Course, CourseId>, ICourseRepository
{
    public CourseRepository(IDocumentSession documentSession) : base(documentSession)
    {
    }
}

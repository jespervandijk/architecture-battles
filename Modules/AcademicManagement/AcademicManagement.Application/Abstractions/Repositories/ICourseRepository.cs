using AcademicManagement.Domain.Aggregates.Courses;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface ICourseRepository : IRepository<Course, CourseId>
{
}

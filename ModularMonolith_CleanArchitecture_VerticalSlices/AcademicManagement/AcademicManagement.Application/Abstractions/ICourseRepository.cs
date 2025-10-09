using AcademicManagement.Domain.Courses;

namespace AcademicManagement.Application.Abstractions;

public interface ICourseRepository : IRepository<Course, CourseId>
{
}

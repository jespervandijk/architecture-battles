using StudentEnrollment.Domain.Courses;

namespace StudentEnrollment.Application.Abstractions.Repositories;

public interface ICourseRepository : IRepository<Course, CourseId>
{
}

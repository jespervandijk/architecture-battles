using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Factories;

public static class CourseFactory
{
    public static Course Create(
            University university,
            Department department,
            Professor courseOwner,
            Name title,
            Credits credits,
            Description? description = null,
            StudentCapacity? maxCapacity = null
        )
    {
        if (courseOwner.DepartmentId != department.Id)
        {
            throw new InvalidOperationException("Course owner must belong to the same department as the course.");
        }

        if (courseOwner.WorkPlace != university.Id)
        {
            throw new InvalidOperationException("Course owner must belong to the same university as the course.");
        }

        if (department.UniversityId != university.Id)
        {
            throw new InvalidOperationException("Department must belong to the specified university.");
        }

        return new Course(
            CourseId.Next(),
            university.Id,
            department.Id,
            courseOwner.Id,
            [courseOwner.Id],
            title,
            description,
            credits,
            maxCapacity,
            CourseStatus.Active,
            []
        );
    }
}

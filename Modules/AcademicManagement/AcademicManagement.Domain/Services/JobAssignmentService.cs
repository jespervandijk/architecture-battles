using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Services;

public static class JobAssignmentService
{
    public static void AddProfessorAsCourseOwner(Course course, Professor professor)
    {
        professor.GuardProfessorWorksAtDepartment(course.Department);
        course.CourseOwner = professor.Id;
        if (!course.Professors.Contains(professor.Id))
        {
            course.AddProfessor(professor.Id);
        }
    }

    public static void AddProfessorToCourse(Course course, Professor professor)
    {
        professor.GuardProfessorWorksAtDepartment(course.Department);
        if (!course.Professors.Contains(professor.Id))
        {
            course.AddProfessor(professor.Id);
        }
    }

    public static void AssignProfessorToDepartment(Professor professor, Department department, bool asHeadOfDepartment = false)
    {
        if (professor.WorkPlace != department.UniversityId)
        {
            throw new InvalidOperationException("Professor can only be assigned to a department in their workplace university");
        }

        professor.AssignToDepartment(department.Id);

        if (asHeadOfDepartment)
        {
            department.HeadOfDepartment = professor.Id;
        }
    }

    private static void GuardProfessorWorksAtDepartment(this Professor professor, DepartmentId departmentId)
    {
        if (professor.DepartmentId != departmentId)
        {
            throw new InvalidOperationException("Professor must belong to the same department as the course.");
        }
    }
}

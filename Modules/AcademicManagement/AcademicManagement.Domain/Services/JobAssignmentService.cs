using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;

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

    public static void AssignHeadOfDepartment(Department department, Professor professor, University university)
    {
        if (department.UniversityId != university.Id)
        {
            throw new InvalidOperationException("Department must belong to the specified university.");
        }
        if (professor.WorkPlace != university.Id)
        {
            throw new InvalidOperationException("Professor must work at the university to be head of department");
        }

        department.HeadOfDepartment = professor.Id;
    }

    private static void GuardProfessorWorksAtDepartment(this Professor professor, DepartmentId departmentId)
    {
        if (professor.DepartmentId != departmentId)
        {
            throw new InvalidOperationException("Professor must belong to the same department as the course.");
        }
    }
}

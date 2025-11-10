using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Domain.Aggregates.Courses;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;

namespace AcademicManagement.Application.Validation;

public static class AuthorizationRules
{
    public static async Task<bool> UserIsPresidentOfUniversity(
        IUserContextService userContext,
        IUniversityRepository universityRepo,
        UniversityId universityId)
    {
        var currentUser = userContext.GetCurrentUser();
        var university = await universityRepo.GetByIdAsync(universityId);
        var presidentId = PresidentId.From(currentUser.Id.Value);

        return university?.President == presidentId;
    }

    public static async Task<bool> UserIsPresidentOfDepartmentUniversity(
        IUserContextService userContext,
        IDepartmentRepository departmentRepo,
        IUniversityRepository universityRepo,
        DepartmentId departmentId)
    {
        var currentUser = userContext.GetCurrentUser();
        var department = await departmentRepo.GetByIdAsync(departmentId);
        if (department is null)
        {
            return false;
        }

        var university = await universityRepo.GetByIdAsync(department.UniversityId);
        var presidentId = PresidentId.From(currentUser.Id.Value);

        return university?.President == presidentId;
    }

    public static async Task<bool> UserIsPresidentOfProfessorUniversity(
        IUserContextService userContext,
        IProfessorRepository professorRepo,
        IUniversityRepository universityRepo,
        ProfessorId professorId)
    {
        var currentUser = userContext.GetCurrentUser();
        var professor = await professorRepo.GetByIdAsync(professorId);
        if (professor is null)
        {
            return false;
        }

        var university = await universityRepo.GetByIdAsync(professor.WorkPlace);
        var presidentId = PresidentId.From(currentUser.Id.Value);

        return university?.President == presidentId;
    }

    public static async Task<bool> UserIsHeadOfCourseDepartment(
        IUserContextService userContext,
        ICourseRepository courseRepo,
        IDepartmentRepository departmentRepo,
        CourseId courseId)
    {
        var currentUser = userContext.GetCurrentUser();
        var course = await courseRepo.GetByIdAsync(courseId);
        if (course is null)
        {
            return false;
        }

        var department = await departmentRepo.GetByIdAsync(course.Department);
        var professorId = ProfessorId.From(currentUser.Id.Value);

        return department?.HeadOfDepartment == professorId;
    }

    public static async Task<bool> UserIsCourseOwner(
        IUserContextService userContext,
        ICourseRepository courseRepo,
        CourseId courseId)
    {
        var currentUser = userContext.GetCurrentUser();
        var course = await courseRepo.GetByIdAsync(courseId);
        var professorId = ProfessorId.From(currentUser.Id.Value);

        return course?.CourseOwner == professorId;
    }

    public static bool UserIsPresident(
        IUserContextService userContext,
        PresidentId presidentId)
    {
        var currentUser = userContext.GetCurrentUser();
        var currentPresidentId = PresidentId.From(currentUser.Id.Value);

        return presidentId == currentPresidentId;
    }

    public static async Task<bool> UserIsSectionProfessor(
        IUserContextService userContext,
        ICourseRepository courseRepo,
        CourseId courseId,
        SectionId sectionId)
    {
        var currentUser = userContext.GetCurrentUser();
        var course = await courseRepo.GetByIdAsync(courseId);
        var section = course?.Sections.FirstOrDefault(s => s.Id == sectionId);
        var professorId = ProfessorId.From(currentUser.Id.Value);

        return section?.Professor == professorId;
    }
}

using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Professors;

public static class ProfessorFactory
{
    public static Professor Create(
        University university,
        Department? department,
        Name firstName,
        Name lastName,
        EmailAddress emailAddress,
        Rank rank,
        UserId userId)
    {
        if (department is not null && department.UniversityId != university.Id)
        {
            throw new InvalidOperationException("Department must belong to the same university as the professor's workplace");
        }

        return new Professor(
            ProfessorId.Next(),
            firstName,
            lastName,
            emailAddress,
            rank,
            university.Id,
            userId,
            department?.Id);
    }
}

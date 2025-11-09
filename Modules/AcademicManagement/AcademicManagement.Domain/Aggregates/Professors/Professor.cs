using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Professors;

public class Professor
{
    public ProfessorId Id { get; private set; }

    public UserId UserId { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public EmailAddress EmailAddress { get; private set; }

    public UniversityId WorkPlace { get; private set; }

    public DepartmentId Department { get; private set; }

    public Rank Rank { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private Professor() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private Professor(string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace, UserId userId)
    {
        Id = ProfessorId.Next();
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Rank = rank;
        WorkPlace = workPlace;
        UserId = userId;
    }

    public static Professor Create(string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace, UserId userId)
    {
        return new Professor(firstName, lastName, emailAddress, rank, workPlace, userId);
    }

    public void ChangeRank(Rank rank)
    {
        Rank = rank;
    }
}

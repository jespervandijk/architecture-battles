using AcademicManagement.Domain.Universities;
using Qowaiv;

namespace AcademicManagement.Domain.Professors;

public class Professor
{
    public ProfessorId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public EmailAddress EmailAddress { get; private set; }

    public UniversityId WorkPlace { get; private set; }

    public Rank Rank { get; private set; }

    private Professor(string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace)
    {
        Id = ProfessorId.Next();
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Rank = rank;
        WorkPlace = workPlace;
    }

    public static Professor Create(string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace)
    {
        return new Professor(firstName, lastName, emailAddress, rank, workPlace);
    }

    public void ChangeRank(Rank rank)
    {
        Rank = rank;
    }
}

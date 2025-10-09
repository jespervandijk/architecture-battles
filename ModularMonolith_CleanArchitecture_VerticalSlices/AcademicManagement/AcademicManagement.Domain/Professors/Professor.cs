using Qowaiv;

namespace AcademicManagement.Domain.Professors;

public class Professor
{
    public ProfessorId Id { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public EmailAddress EmailAddress { get; set; }

    public Rank Rank { get; set; }
}

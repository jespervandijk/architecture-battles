using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Professors;

public sealed class Professor
{
    public ProfessorId Id { get; init; }
    public UserId UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public EmailAddress EmailAddress { get; set; }
    public UniversityId WorkPlace { get; set; }
    public DepartmentId? DepartmentId { get; set; }
    public Rank Rank { get; set; }

    [JsonConstructor]
    private Professor(ProfessorId id, string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace, UserId userId, DepartmentId? departmentId)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Rank = rank;
        WorkPlace = workPlace;
        UserId = userId;
        DepartmentId = departmentId;
    }

    public static Professor Create(string firstName, string lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace, UserId userId, DepartmentId? departmentId = null)
    {
        return new Professor(ProfessorId.Next(), firstName, lastName, emailAddress, rank, workPlace, userId, departmentId);
    }
}

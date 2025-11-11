using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;
using Qowaiv;

namespace AcademicManagement.Domain.Aggregates.Professors;

public sealed class Professor
{
    public ProfessorId Id { get; init; }
    public UserId UserId { get; init; }
    public Name FirstName { get; private set; }
    public Name LastName { get; private set; }
    public EmailAddress EmailAddress { get; private set; }
    public UniversityId WorkPlace { get; init; }
    public DepartmentId? DepartmentId { get; private set; }
    public Rank Rank { get; private set; }

    [JsonConstructor]
    internal Professor(ProfessorId id, Name firstName, Name lastName, EmailAddress emailAddress, Rank rank, UniversityId workPlace, UserId userId, DepartmentId? departmentId)
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

    public void Update(Name firstName, Name lastName, EmailAddress emailAddress, Rank rank)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Rank = rank;
    }

    public void AssignToDepartment(DepartmentId departmentId)
    {
        DepartmentId = departmentId;
    }
}

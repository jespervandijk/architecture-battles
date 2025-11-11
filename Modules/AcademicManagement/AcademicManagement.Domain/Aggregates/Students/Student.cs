using System.Text.Json.Serialization;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Students;

public sealed class Student
{
    public StudentId Id { get; init; }
    public Name FirstName { get; private set; }
    public Name LastName { get; private set; }

    [JsonConstructor]
    private Student(StudentId id, Name firstName, Name lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    public static Student Create(Name firstName, Name lastName)
    {
        return new Student(StudentId.Next(), firstName, lastName);
    }
}

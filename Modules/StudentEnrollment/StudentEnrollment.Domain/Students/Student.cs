
using Qowaiv;

namespace StudentEnrollment.Domain.Students;

public class Student
{
    public StudentId Id { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public EmailAddress EmailAddress { get; private set; }

    public Student(string firstName, string lastName, EmailAddress emailAddress)
    {
        Id = StudentId.Next();
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
    }
}


using System.Text.Json.Serialization;
using AcademicManagement.Domain.Aggregates.Users;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Presidents;

public sealed class President
{
    public PresidentId Id { get; private set; }

    public UserId UserId { get; private set; }

    public Name FirstName { get; set; }

    public Name LastName { get; set; }


    [JsonConstructor]
    private President(PresidentId id, UserId userId, Name firstName, Name lastName)
    {
        Id = id;
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
    }

    public static President Create(UserId userId, Name firstName, Name lastName)
    {
        return new President(PresidentId.Next(), userId, firstName, lastName);
    }
}

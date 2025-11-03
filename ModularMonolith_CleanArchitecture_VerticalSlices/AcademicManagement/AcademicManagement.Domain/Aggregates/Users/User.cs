
namespace AcademicManagement.Domain.Aggregates.Users;

public record User
{
    public UserId Id { get; init; }
    public UserName Name { get; init; }

    public UserRole Role { get; init; }

    private User(UserId id, UserName name, UserRole role)
    {
        Id = id;
        Name = name;
        Role = role;
    }

    public static User Create(UserName name, UserRole role)
    {
        return new User(UserId.Next(), name, role);
    }

    public static User FromClaims(string id, string name, string role)
    {
        return new User(UserId.From(Guid.Parse(id)), UserName.From(name), UserRole.From(role));
    }
}

namespace AcademicManagement.Domain.GeneralValueObjects.Users;

public record User
{
    public UserName Name { get; init; }

    public UserRole Role { get; init; }

    public User(UserName name, UserRole role)
    {
        Name = name;
        Role = role;
    }
}

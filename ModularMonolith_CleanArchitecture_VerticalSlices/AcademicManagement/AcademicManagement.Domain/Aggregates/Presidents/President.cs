
using AcademicManagement.Domain.Aggregates.Users;

namespace AcademicManagement.Domain.Aggregates.Presidents;

public class President
{
    public PresidentId Id { get; private set; }

    public UserId UserId { get; private set; }


    private President(PresidentId id, UserId userId)
    {
        Id = id;
        UserId = userId;
    }

    public static President Create(UserId userId)
    {
        return new President(PresidentId.Next(), userId);
    }
}

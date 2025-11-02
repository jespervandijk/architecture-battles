
using AcademicManagement.Domain.GeneralValueObjects.Users;

namespace AcademicManagement.Domain.Aggregates.Presidents;

public class President
{
    public PresidentId Id { get; private set; }

    public User UserData { get; private set; }

    private President() { }
}

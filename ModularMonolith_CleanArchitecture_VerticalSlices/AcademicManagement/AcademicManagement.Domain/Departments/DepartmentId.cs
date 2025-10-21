using Vogen;

namespace AcademicManagement.Domain.Departments;

[ValueObject<Guid>]
public partial struct DepartmentId
{
    public static DepartmentId Next() => From(Guid.NewGuid());
}

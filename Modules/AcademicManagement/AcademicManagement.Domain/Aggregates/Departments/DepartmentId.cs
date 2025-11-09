using Vogen;

namespace AcademicManagement.Domain.Aggregates.Departments;

[ValueObject<Guid>]
public partial struct DepartmentId
{
    public static DepartmentId Next() => From(Guid.NewGuid());
}

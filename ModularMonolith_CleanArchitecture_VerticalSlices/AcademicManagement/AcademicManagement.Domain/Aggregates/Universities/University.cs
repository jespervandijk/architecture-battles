using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Universities;

public class University
{
    public UniversityId Id { get; private set; }

    public PresidentId President { get; private set; }

    public Name Name { get; set; }

    public List<DepartmentId> Departments { get; private set; }

    public List<ProfessorId> Professors { get; private set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    private University() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    private University(PresidentId president, Name name)
    {
        Id = UniversityId.Next();
        Professors = [];
        Departments = [];
        President = president;
        Name = name;
    }

    public static University Create(PresidentId president, Name name)
    {
        return new University(president, name);
    }
}

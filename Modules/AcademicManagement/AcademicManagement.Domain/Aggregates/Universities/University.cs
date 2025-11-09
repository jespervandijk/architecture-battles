using AcademicManagement.Domain.Aggregates.Departments;
using AcademicManagement.Domain.Aggregates.Presidents;
using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Universities;

public sealed class University
{
    public UniversityId Id { get; init; }

    public PresidentId President { get; set; }

    public Name Name { get; set; }

    public List<DepartmentId> Departments { get; set; }

    public List<ProfessorId> Professors { get; set; }

    private University(UniversityId id, PresidentId president, Name name, List<DepartmentId> departments, List<ProfessorId> professors)
    {
        Id = id;
        President = president;
        Name = name;
        Departments = departments;
        Professors = professors;
    }

    public static University Create(PresidentId president, Name name)
    {
        return new University(UniversityId.Next(), president, name, [], []);
    }
}

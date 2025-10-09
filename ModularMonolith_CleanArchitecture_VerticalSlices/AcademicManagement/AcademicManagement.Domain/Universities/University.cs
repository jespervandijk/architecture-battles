using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Domain.Universities;

public class University
{
    public required UniversityId Id { get; set; }

    public required List<Department> Departments { get; set; }

    public required List<ProfessorId> Professors { get; set; }
}

using AcademicManagement.Domain.Aggregates.Professors;

namespace AcademicManagement.Domain.Aggregates.Courses;

public class Section
{
    public string Name { get; set; }

    public ProfessorId Professor { get; set; }
    public List<Assignment> Assignments { get; set; }

    public List<Test> Tests { get; set; }

}

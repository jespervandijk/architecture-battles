
using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Domain.Universities;

public class Department
{
    public required string Name { get; set; }

    public required ProfessorId HeadOfDepartment { get; set; }

}

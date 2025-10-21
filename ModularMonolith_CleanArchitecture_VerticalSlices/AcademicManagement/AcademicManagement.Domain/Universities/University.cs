using AcademicManagement.Domain.Departments;
using AcademicManagement.Domain.Presidents;
using AcademicManagement.Domain.Professors;

namespace AcademicManagement.Domain.Universities;

public class University
{
    public UniversityId Id { get; private set; }

    public PresidentId President { get; private set; }

    public string Name { get; set; }

    // public List<Department> Departments { get; private set; }

    // public List<ProfessorId> Professors { get; private set; }

    private University(PresidentId president, string name)
    {
        Id = UniversityId.Next();
        // Professors = [];
        // Departments = [];
        President = president;
        Name = name;
    }

    // public static University Create(PresidentId president, string name)
    // {
    //     return new University(president, name);
    // }

    // public void AddDepartment(string name, ProfessorId headOfDepartment)
    // {
    //     if (Departments.Exists(x => x.Name == name))
    //     {
    //         throw new ArgumentNullException("Department with this name already exists");
    //     }
    //     Departments.Add(Department.Create(name, headOfDepartment));
    // }

    // public void ChangeHeadOfDepartment(string name, ProfessorId headOfDepartment)
    // {
    //     Departments = [.. Departments.Select(department =>
    //         {
    //             if (department.Name == name){
    //                 return department with { HeadOfDepartment = headOfDepartment };
    //             }
    //             return department;
    //         })];
    // }

    // public void RemoveDepartment(string name)
    // {
    //     Departments = [.. Departments.Where(x => x.Name != name)];
    // }
}

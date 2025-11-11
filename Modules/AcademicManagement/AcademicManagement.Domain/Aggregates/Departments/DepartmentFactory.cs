using AcademicManagement.Domain.Aggregates.Professors;
using AcademicManagement.Domain.Aggregates.Universities;
using AcademicManagement.Domain.Scalars;

namespace AcademicManagement.Domain.Aggregates.Departments;

public static class DepartmentFactory
{
    public static Department Create(University university, Professor headOfDepartment, Name name)
    {
        if (headOfDepartment.WorkPlace != university.Id)
        {
            throw new InvalidOperationException("Professor must work at the university to be head of department");
        }

        return new Department(DepartmentId.Next(), university.Id, name, headOfDepartment.Id, false);
    }
}

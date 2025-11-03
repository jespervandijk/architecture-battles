namespace AcademicManagement.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync();
}

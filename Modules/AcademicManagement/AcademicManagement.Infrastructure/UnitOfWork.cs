using AcademicManagement.Application.Abstractions;
using Marten;

namespace AcademicManagement.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly IDocumentSession _documentSession;

    public UnitOfWork(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task SaveChangesAsync()
    {
        await _documentSession.SaveChangesAsync();
    }
}

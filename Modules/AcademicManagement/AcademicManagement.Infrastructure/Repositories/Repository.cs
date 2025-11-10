using AcademicManagement.Application.Abstractions.Repositories;
using AcademicManagement.Application.Exceptions;
using Marten;
using Marten.Linq;

namespace AcademicManagement.Infrastructure.Repositories;

public abstract class Repository<TEntity, TId> : IRepository<TEntity, TId>
    where TEntity : notnull
    where TId : notnull
{
    private readonly IDocumentSession _documentSession;


    public Repository(IDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<IReadOnlyList<TEntity>> GetAllAsync()
    {
        return await _documentSession.Query<TEntity>().ToListAsync(); ;
    }

    public IMartenQueryable<TEntity> Query()
    {
        return _documentSession.Query<TEntity>();
    }

    public async Task<TEntity> GetByIdAsync(TId id)
    {
        return await _documentSession.LoadAsync<TEntity>(id) ?? throw new EntityNotFoundException(nameof(TEntity), id);
    }

    public async Task<TEntity?> TryGetByIdAsync(TId id)
    {
        return await _documentSession.LoadAsync<TEntity>(id);
    }

    public async Task<bool> ExistsAsync(TId id)
    {
        var entity = await _documentSession.LoadAsync<TEntity>(id);
        return entity is not null;
    }

    public void Update(TEntity entity)
    {
        _documentSession.Update(entity);
    }

    public void Delete(TId id)
    {
        _documentSession.Delete(id);
    }

    public void Insert(TEntity entity)
    {
        _documentSession.Insert(entity);
    }

    public void Upsert(TEntity entity)
    {
        _documentSession.Store(entity);
    }
}

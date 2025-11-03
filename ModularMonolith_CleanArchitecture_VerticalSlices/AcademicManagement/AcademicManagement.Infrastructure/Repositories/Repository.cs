using AcademicManagement.Application.Abstractions.Repositories;
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

    public async Task<IReadOnlyList<TEntity>> GetAll()
    {
        return await _documentSession.Query<TEntity>().ToListAsync(); ;
    }

    public IMartenQueryable<TEntity> Query()
    {
        return _documentSession.Query<TEntity>();
    }

    public async Task<TEntity> GetById(TId id)
    {
        return await _documentSession.LoadAsync<TEntity>(id) ?? throw new KeyNotFoundException($"Entity of type {typeof(TEntity).Name} with id {id} was not found.");
    }

    public async Task Update(TEntity entity)
    {
        _documentSession.Update(entity);
        await _documentSession.SaveChangesAsync();
    }

    public async Task Delete(TId id)
    {
        _documentSession.Delete(id);
        await _documentSession.SaveChangesAsync();
    }

    public async Task Create(TEntity entity)
    {
        _documentSession.Insert(entity);
        await _documentSession.SaveChangesAsync();
    }

    public async Task Upsert(TEntity entity)
    {
        _documentSession.Store(entity);
        await _documentSession.SaveChangesAsync();
    }
}

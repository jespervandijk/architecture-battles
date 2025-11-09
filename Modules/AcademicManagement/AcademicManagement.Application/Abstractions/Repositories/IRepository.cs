using Marten.Linq;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IRepository<TEntity, TId>
{
    public Task<IReadOnlyList<TEntity>> GetAllAsync();

    public Task<TEntity> GetByIdAsync(TId id);

    public void Update(TEntity entity);

    public void Delete(TId id);

    public void Insert(TEntity entity);

    public void Upsert(TEntity entity);

    public IMartenQueryable<TEntity> Query();
}
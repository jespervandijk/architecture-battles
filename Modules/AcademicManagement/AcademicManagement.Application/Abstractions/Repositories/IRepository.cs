using Marten.Linq;

namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IRepository<TEntity, TId>
{
    Task<IReadOnlyList<TEntity>> GetAllAsync();

    Task<TEntity> GetByIdAsync(TId id);

    Task<TEntity?> TryGetByIdAsync(TId id);

    Task<bool> ExistsAsync(TId id);

    void Update(TEntity entity);

    void Delete(TId id);

    void Insert(TEntity entity);

    void Upsert(TEntity entity);

    IMartenQueryable<TEntity> Query();
}
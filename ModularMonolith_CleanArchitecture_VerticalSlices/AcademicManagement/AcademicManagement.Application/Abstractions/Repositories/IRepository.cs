namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IRepository<TEntity, TId>
{
    public Task<IReadOnlyList<TEntity>> GetAll();

    public Task<TEntity> GetById(TId id);

    public Task Update(TEntity entity);

    public Task Delete(TId id);

    public Task Create(TEntity entity);
}
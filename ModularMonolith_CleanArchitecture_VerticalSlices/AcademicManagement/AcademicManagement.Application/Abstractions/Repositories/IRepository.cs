namespace AcademicManagement.Application.Abstractions.Repositories;

public interface IRepository<TEntity, TId>
{
    public Task<List<TEntity>> GetAll();

    public Task<TEntity> GetById(TId id);

    public Task Update(TEntity entity);

    public Task Delete(TEntity entity);

    public Task Create(TEntity entity);
}
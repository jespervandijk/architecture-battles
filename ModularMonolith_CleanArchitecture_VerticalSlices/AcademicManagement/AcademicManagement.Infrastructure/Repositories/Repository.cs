namespace AcademicManagement.Infrastructure.Repositories;

public abstract class Repository<TEntity, TId>
{
    public async Task<List<TEntity>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<TEntity> GetById(TId id)
    {
        throw new NotImplementedException();
    }

    public async Task Update(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task Delete(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public async Task Create(TEntity entity)
    {
        throw new NotImplementedException();
    }
}

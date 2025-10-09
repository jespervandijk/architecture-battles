namespace StudentEnrollment.Infrastructure.Repositories;

public abstract class Repository<TEntity, TId>
{
    public List<TEntity> GetAll()
    {
        throw new NotImplementedException();
    }

    public TEntity GetById(TId id)
    {
        throw new NotImplementedException();
    }

    public void Update(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public void Delete(TEntity entity)
    {
        throw new NotImplementedException();
    }
}

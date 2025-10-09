namespace AcademicManagement.Application.Abstractions;

public interface IRepository<TEntity, TId>
{
    public List<TEntity> GetAll();

    public TEntity GetById(TId id);

    public void Update(TEntity entity);

    public void Delete(TEntity entity);
}
using System.Linq.Expressions;

namespace MaintenanceApp.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    IEnumerable<TEntity> GetAll();
    TEntity GetById(Guid id);
    TEntity Add(TEntity entity);
    TEntity Update(TEntity entity);
    TEntity Delete(TEntity entity);
    int Count();
    IQueryable<TEntity> GetQuery();
    IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter);
    IQueryable<TEntity> GetQuery(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
        string includeProperties = "");
}
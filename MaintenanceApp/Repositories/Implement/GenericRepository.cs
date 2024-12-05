using System.Linq.Expressions;
using MaintenanceApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceApp.Repositories.Implement;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    
    public IEnumerable<TEntity> GetAll()
    {
        return _dbSet.ToList();
    }
    
    public TEntity GetById(Guid id)
    {
        return _dbSet.Find(id);
    }

    public TEntity Add(TEntity entity)
    {
        return _dbSet.Add(entity).Entity;
    }

    public TEntity Update(TEntity entity)
    {
        return _dbSet.Update(entity).Entity;
    }

    public TEntity Delete(TEntity entity)
    {
        return _dbSet.Remove(entity).Entity;
    }

    public IEnumerable<TEntity> GetPaged(int pageNumber, int pageSize)
    {
        return _dbSet.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
    }

    public int Count()
    {
        return _dbSet.Count();
    }

    public IQueryable<TEntity> GetQuery()
    {
        return _dbSet;
    }

    public IQueryable<TEntity> GetQuery(Expression<Func<TEntity, bool>> filter)
    {
        return _dbSet.Where(filter);
    }

    
    public IQueryable<TEntity> GetQuery(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "")
    {
        IQueryable<TEntity> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            foreach (string includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        return orderBy != null ? orderBy(query) : query;
    }
    
   
}
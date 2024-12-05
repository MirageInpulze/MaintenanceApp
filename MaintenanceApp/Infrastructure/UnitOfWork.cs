using MaintenanceApp.Data;
using MaintenanceApp.Repositories;
using MaintenanceApp.Repositories.Implement;

namespace MaintenanceApp.Infrastructure;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly Dictionary<Type, object> _repositories = [];
    
    public IGenericRepository<T> GetRepository<T>() where T : class
    {
        _repositories.TryGetValue(typeof(T), out var repo);
        
        if (repo != null) return (GenericRepository<T>) repo;
        
        repo = new GenericRepository<T>(context);
        _repositories.Add(typeof(T), repo);
        
        return (GenericRepository<T>) repo;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
    
    public AppDbContext GetDbContext()
    {
        return context;
    }
}
using MaintenanceApp.Data;
using MaintenanceApp.Repositories;

namespace MaintenanceApp.Infrastructure;

public interface IUnitOfWork
{
    public IGenericRepository<T> GetRepository<T>() where T : class;

    public Task<int> SaveChangesAsync();

    public AppDbContext GetDbContext();
}
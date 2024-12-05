using System.Linq.Expressions;
using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services.Implement;

public interface IServiceBase<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync(TEntity entity);
    Task<TEntity> DeleteAsync(Guid id);
    Task<TEntity> DeleteLogicAsync(Guid id);
    Task<TEntity> GetByIdAsync(Guid id);
    Task<PaginatedResult<TEntity>> GetAsync(QueryParametersVM parameters);
}
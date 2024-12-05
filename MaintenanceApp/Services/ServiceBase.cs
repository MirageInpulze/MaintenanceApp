using System.Linq.Expressions;
using System.Text.Json;
using MaintenanceApp.Exceptions;
using MaintenanceApp.Infrastructure;
using Microsoft.EntityFrameworkCore;
//using MaintenanceApp.Constant;
using MaintenanceApp.Exceptions;
using MaintenanceApp.Infrastructure;
using MaintenanceApp.Services.Implement;
using MaintenanceApp.ViewModels;

//using MaintenanceApp.Services.Implement;
//using MaintenanceApp.ViewModels;

namespace MaintenanceApp.Services;

public class ServiceBase<TEntity>(IUnitOfWork unitOfWork, ILogger<ServiceBase<TEntity>> logger)
    : IServiceBase<TEntity>
    where TEntity : class
{
    public virtual async Task<TEntity> CreateAsync(TEntity entity)
    {
        if (entity == null)
        {
            logger.LogError($"{nameof(TEntity)} is null!");
            throw new ArgumentNullException(nameof(entity));
        }

        unitOfWork.GetRepository<TEntity>().Add(entity);

        var result = await unitOfWork.SaveChangesAsync();

        if (result > 0)
        {
            logger.LogInformation($"{nameof(TEntity)} added successfully!");
            return entity; // Trả về entity đã thêm
        }
        else
        {
            logger.LogError($"{nameof(TEntity)} add failed!");
            throw new BadRequestException($"{nameof(TEntity)} add failed!");
        }
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null)
        {
            logger.LogError("Entity is null!");
            throw new ArgumentNullException(nameof(entity));
        }

        // Kiểm tra và tách thực thể nếu nó đã được theo dõi
        await DetachIfEntityExistsAsync(entity);

        unitOfWork.GetRepository<TEntity>().Update(entity);

        var result = await unitOfWork.SaveChangesAsync();

        if (result > 0)
        {
            logger.LogInformation("Update successfully!");
            return entity; // Trả về entity đã cập nhật
        }
        else
        {
            logger.LogError("Update failed!");
            throw new BadRequestException("Update failed!");
        }
    }

    private async Task DetachIfEntityExistsAsync(TEntity entity)
    {
        // Dùng Reflection để kiểm tra xem 'Id' có tồn tại trong TEntity không
        var idProperty = typeof(TEntity).GetProperty("Id");
        if (idProperty == null)
        {
            logger.LogError("Entity does not have an Id property!");
            return; // Nếu không có thuộc tính Id, không làm gì thêm
        }

        // Lấy giá trị của Id từ entity
        var entityId = idProperty.GetValue(entity);

        if (entityId == null)
        {
            logger.LogError("Entity Id is null!");
            return; // Nếu Id là null, không làm gì thêm
        }

        // Tìm thực thể đã tồn tại trong cơ sở dữ liệu với Id tương tự
        var existingEntity = unitOfWork.GetRepository<TEntity>()
            .GetAll()
            .FirstOrDefault(v => idProperty.GetValue(v).Equals(entityId));

        if (existingEntity != null)
        {
            unitOfWork.GetDbContext().Entry(existingEntity).State = EntityState.Detached;
            logger.LogInformation($"Detached entity with Id {entityId} to avoid tracking conflicts.");
        }
    }

    public virtual async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity == null)
        {
            logger.LogError("Entity is null!");
            throw new ArgumentNullException(nameof(entity));
        }

        unitOfWork.GetRepository<TEntity>().Delete(entity);

        var result = await unitOfWork.SaveChangesAsync();

        if (result > 0)
        {
            logger.LogInformation("Delete successfully!");
            return entity; // Trả về entity đã xóa
        }
        else
        {
            logger.LogError("Delete failed!");
            throw new BadRequestException("Delete failed!");
        }
    }

    public virtual async Task<TEntity> DeleteAsync(Guid id)
    {
        TEntity entityDelete = unitOfWork.GetRepository<TEntity>().GetById(id);

        if (entityDelete == null)
        {
            logger.LogError($"{nameof(TEntity)} is null!");
            throw new ArgumentNullException(nameof(entityDelete));
        }

        unitOfWork.GetRepository<TEntity>().Delete(entityDelete);

        var result = await unitOfWork.SaveChangesAsync();

        if (result > 0)
        {
            logger.LogInformation($"{nameof(TEntity)} delete successfully!");
            return entityDelete; // Trả về entity đã xóa
        }
        else
        {
            logger.LogError($"{nameof(TEntity)} Delete failed!");
            throw new BadRequestException($"{nameof(TEntity)} Delete failed!");
        }
    }

    public virtual async Task<TEntity> DeleteLogicAsync(Guid id)
    {
        // Lấy thực thể từ repository theo id
        TEntity entity = unitOfWork.GetRepository<TEntity>().GetById(id);
        if (entity == null)
        {
            logger.LogError($"{nameof(entity)} is null!");
            throw new ArgumentNullException(nameof(entity));
        }

        // Sử dụng reflection để kiểm tra và cập nhật thuộc tính IsActive
        var isActiveProperty = typeof(TEntity).GetProperty("IsActive");
        if (isActiveProperty == null || isActiveProperty.PropertyType != typeof(bool))
        {
            logger.LogError($"{nameof(entity)} does not contain an IsActive property!");
            throw new InvalidOperationException($"{nameof(entity)} does not contain an IsActive property.");
        }

        // Đặt IsActive thành false
        isActiveProperty.SetValue(entity, false);

        // Cập nhật thực thể
        entity = unitOfWork.GetRepository<TEntity>().Update(entity);

        // Lưu thay đổi và kiểm tra kết quả
        if (await unitOfWork.SaveChangesAsync() > 0)
        {
            logger.LogInformation($"{nameof(entity)} DeleteLogic successfully!");
            return entity; // Trả về thực thể đã "xóa" logic
        }
        else
        {
            logger.LogError($"{nameof(entity)} DeleteLogic failed!");
            throw new BadRequestException($"{nameof(entity)} Delete failed!");
        }
    }

    public virtual async Task<TEntity> GetByIdAsync(Guid id)
    {
        TEntity entity = unitOfWork.GetRepository<TEntity>().GetById(id);
        if (entity != null)
        {
            logger.LogInformation($"{nameof(TEntity)} delete successfully!");
            return entity; // Trả về entity đã xóa
        }
        else
        {
            logger.LogError($"{nameof(TEntity)} Delete failed!");
            return null;
        }
    }

    public virtual async Task<PaginatedResult<TEntity>> GetAsync(
        int pageIndex = 1, int pageSize = 10,
        string includeProperties = "",
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null)
    {
        var query = unitOfWork.GetRepository<TEntity>()
            .GetQuery(filter, orderBy, includeProperties);

        // Áp dụng bộ lọc IsActive = true
        query = ApplyIsActiveFilter(query);

        return await PaginatedResult<TEntity>.CreateAsync(query, pageIndex, pageSize);
    }

    private static IQueryable<TEntity> ApplyIsActiveFilter(IQueryable<TEntity> query)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "e");
        var isActiveProperty = Expression.PropertyOrField(parameter, "IsActive");
        var condition = Expression.Equal(isActiveProperty, Expression.Constant(true));
        var lambda = Expression.Lambda<Func<TEntity, bool>>(condition, parameter);

        return query.Where(lambda);
    }

    // public async Task<PaginatedResult<TEntity>> GetAsync(QueryParametersVM parameters)
    // {
    //     Expression<Func<TEntity, bool>>? filter = null;
    //
    //     if (parameters.Filter != null)
    //     {
    //         var parameter = Expression.Parameter(typeof(TEntity), "entity");
    //         Expression? filterExpression = null;
    //
    //         foreach (var kvp in parameters.Filter)
    //         {
    //             var propertyName = kvp.Key;
    //             var value = kvp.Value;
    //
    //             var property = typeof(TEntity).GetProperty(propertyName);
    //             if (property != null)
    //             {
    //                 var propertyAccess = Expression.Property(parameter, property);
    //                 Expression constant;
    //
    //                 // Chuyển đổi giá trị JsonElement thành kiểu dữ liệu tương ứng
    //                 if (value is JsonElement jsonElement)
    //                 {
    //                     switch (jsonElement.ValueKind)
    //                     {
    //                         case JsonValueKind.String:
    //                             if (property.PropertyType == typeof(Guid))
    //                             {
    //                                 Guid guidValue;
    //                                 if (Guid.TryParse(jsonElement.GetString(), out guidValue))
    //                                 {
    //                                     constant = Expression.Constant(guidValue);
    //                                 }
    //                                 else
    //                                 {
    //                                     continue; // Bỏ qua nếu không thể chuyển đổi
    //                                 }
    //                             }
    //                             else
    //                             {
    //                                 constant = Expression.Constant(jsonElement.GetString());
    //                             }
    //
    //                             break;
    //                         case JsonValueKind.Number:
    //                             if (property.PropertyType == typeof(int))
    //                             {
    //                                 constant = Expression.Constant(jsonElement.GetInt32());
    //                             }
    //                             else if (property.PropertyType == typeof(decimal))
    //                             {
    //                                 constant = Expression.Constant(jsonElement.GetDecimal());
    //                             }
    //                             else if (property.PropertyType == typeof(double))
    //                             {
    //                                 constant = Expression.Constant(jsonElement.GetDouble());
    //                             }
    //                             else if (property.PropertyType.IsEnum) // Kiểm tra xem có phải là enum không
    //                             {
    //                                 var enumType = property.PropertyType;
    //                                 var enumValue = jsonElement.GetInt32(); // Lấy giá trị nguyên
    //                                 if (Enum.IsDefined(enumType, enumValue))
    //                                 {
    //                                     constant = Expression.Constant(Enum.ToObject(enumType, enumValue));
    //                                 }
    //                                 else
    //                                 {
    //                                     continue; // Bỏ qua nếu không hợp lệ
    //                                 }
    //                             }
    //                             else
    //                             {
    //                                 constant = Expression.Constant(Convert.ChangeType(jsonElement.GetDouble(),
    //                                     property.PropertyType));
    //                             }
    //
    //                             break;
    //                         case JsonValueKind.True:
    //                         case JsonValueKind.False:
    //                             constant = Expression.Constant(jsonElement.GetBoolean());
    //                             break;
    //                         case JsonValueKind.Null:
    //                             continue; // Bỏ qua trường hợp null
    //                         default:
    //                             continue; // Bỏ qua các loại không hỗ trợ
    //                     }
    //
    //                     var equality = Expression.Equal(propertyAccess, constant);
    //                     filterExpression = filterExpression == null
    //                         ? equality
    //                         : Expression.AndAlso(filterExpression, equality);
    //                 }
    //             }
    //         }
    //
    //         if (filterExpression != null)
    //         {
    //             filter = Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
    //         }
    //     }
    //     
    //     Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
    //     if (!string.IsNullOrEmpty(parameters.OrderBy))
    //     {
    //         orderBy = CreateOrderByFunc(parameters.OrderBy, parameters.SortDirection);
    //     }
    //
    //     return await GetAsync(parameters.PageIndex, parameters.PageSize, parameters.IncludeProperties, filter, orderBy);
    // }

    public async Task<PaginatedResult<TEntity>> GetAsync(QueryParametersVM parameters)
    {
        Expression<Func<TEntity, bool>>? filter = null;

        if (parameters.Filter != null)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "entity");
            Expression? filterExpression = null;

            foreach (var kvp in parameters.Filter)
            {
                var propertyName = kvp.Key;
                var value = kvp.Value;

                var property = typeof(TEntity).GetProperty(propertyName);
                if (property != null)
                {
                    var propertyAccess = Expression.Property(parameter, property);
                    Expression constant;

                    if (value is JsonElement jsonElement)
                    {
                        switch (jsonElement.ValueKind)
                        {
                            case JsonValueKind.String:
                                if (property.PropertyType == typeof(Guid) || property.PropertyType == typeof(Guid?))
                                {
                                    // Kiểm tra và chuyển đổi string thành Guid
                                    if (Guid.TryParse(jsonElement.GetString(), out var guidValue))
                                    {
                                        constant = Expression.Constant(guidValue, property.PropertyType);
                                        var equality = Expression.Equal(propertyAccess, constant);

                                        filterExpression = filterExpression == null
                                            ? equality
                                            : Expression.AndAlso(filterExpression, equality);
                                    }

                                    continue;
                                }
                                else if (property.PropertyType == typeof(string))
                                {
                                    // Tìm kiếm tương đối với Contains
                                    constant = Expression.Constant(jsonElement.GetString(), typeof(string));
                                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                                    var containsExpression = Expression.Call(propertyAccess, containsMethod!, constant);

                                    filterExpression = filterExpression == null
                                        ? containsExpression
                                        : Expression.AndAlso(filterExpression, containsExpression);
                                }
                                else
                                {
                                    constant = Expression.Constant(jsonElement.GetString(), property.PropertyType);
                                    var equality = Expression.Equal(propertyAccess, constant);

                                    filterExpression = filterExpression == null
                                        ? equality
                                        : Expression.AndAlso(filterExpression, equality);
                                }

                                break;

                            case JsonValueKind.Number:
                                if (property.PropertyType == typeof(int))
                                {
                                    constant = Expression.Constant(jsonElement.GetInt32());
                                }
                                else if (property.PropertyType == typeof(decimal))
                                {
                                    constant = Expression.Constant(jsonElement.GetDecimal());
                                }
                                else if (property.PropertyType == typeof(double))
                                {
                                    constant = Expression.Constant(jsonElement.GetDouble());
                                }
                                else if (property.PropertyType.IsEnum)
                                {
                                    var enumType = property.PropertyType;
                                    var enumValue = jsonElement.GetInt32();
                                    if (Enum.IsDefined(enumType, enumValue))
                                    {
                                        constant = Expression.Constant(Enum.ToObject(enumType, enumValue));
                                    }
                                    else
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    constant = Expression.Constant(Convert.ChangeType(jsonElement.GetDouble(),
                                        property.PropertyType));
                                }

                                var equalityNumber = Expression.Equal(propertyAccess, constant);
                                filterExpression = filterExpression == null
                                    ? equalityNumber
                                    : Expression.AndAlso(filterExpression, equalityNumber);

                                break;

                            case JsonValueKind.True:
                            case JsonValueKind.False:
                                constant = Expression.Constant(jsonElement.GetBoolean());
                                var equalityBoolean = Expression.Equal(propertyAccess, constant);
                                filterExpression = filterExpression == null
                                    ? equalityBoolean
                                    : Expression.AndAlso(filterExpression, equalityBoolean);

                                break;

                            case JsonValueKind.Null:
                                continue;

                            default:
                                continue;
                        }
                    }
                }
            }

            if (filterExpression != null)
            {
                filter = Expression.Lambda<Func<TEntity, bool>>(filterExpression, parameter);
            }
        }

        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null;
        if (!string.IsNullOrEmpty(parameters.OrderBy))
        {
            orderBy = CreateOrderByFunc(parameters.OrderBy, parameters.SortDirection);
        }

        return await GetAsync(parameters.PageIndex, parameters.PageSize, parameters.IncludeProperties, filter, orderBy);
    }


    private Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> CreateOrderByFunc(string orderBy,
        string sortDirection)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "entity");
        var property = typeof(TEntity).GetProperty(orderBy);

        if (property == null)
        {
            throw new ArgumentException($"'{orderBy}' is not a valid property of {typeof(TEntity).Name}");
        }

        var propertyAccess = Expression.MakeMemberAccess(parameter, property);
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);

        var typeArguments = new Type[] { typeof(TEntity), property.PropertyType };
        var methodName = sortDirection.Equals("DESC", StringComparison.OrdinalIgnoreCase)
            ? "OrderByDescending"
            : "OrderBy";
        var orderByMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeArguments);

        return query =>
            (IOrderedQueryable<TEntity>)orderByMethod.Invoke(null, new object[] { query, orderByExpression });
    }
}
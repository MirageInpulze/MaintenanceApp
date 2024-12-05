using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceApp.ViewModels;

public class PaginatedResult<TEntity>
    where TEntity : class
{
    public int PageIndex { get; private set; }
    public int TotalPages { get; private set;}
    public int TotalItem { get; private set;}
    public List<TEntity> Items { get; private set; }

    public PaginatedResult(List<TEntity> items,int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling((decimal)count / pageSize);
        TotalItem = count;
        Items = items;

    }

    public bool HasPreviousPage => PageIndex > 1;
        
    public bool HasNextPage => PageIndex < TotalPages;

    public static async Task<PaginatedResult<TEntity>> CreateAsync(IQueryable<TEntity> query, int pageIndex, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedResult<TEntity>(items, count, pageIndex, pageSize);
    }
    
   
}
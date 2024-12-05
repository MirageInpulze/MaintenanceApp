using System.Linq.Expressions;

namespace MaintenanceApp.ViewModels;

public class QueryParametersVM
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string IncludeProperties { get; set; } = string.Empty;

    // For filter and orderBy, you would need to parse expressions or provide a predefined filter and ordering.
    public  Dictionary<string, object>? Filter { get; set; }
    public string? OrderBy { get; set; }
    public string SortDirection { get; set; } = "ASC"; 
    
}
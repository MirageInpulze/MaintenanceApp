using MaintenanceApp.Constant;

namespace MaintenanceApp.ViewModels;

public class UserVm
{
    public Guid? Id { get; set; }
    public string? UserName { get; set; }
    public Boolean IsActive  { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public string? Role { get; set; }
    
    public string? Password { get; set; }
}
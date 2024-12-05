using Microsoft.AspNetCore.Identity;

namespace MaintenanceApp.Models;

public class User: IdentityUser<Guid>
{
    public Boolean IsActive  { get; set; }
    public string? FullName { get; set; }
}
using Microsoft.AspNetCore.Identity;

namespace MaintenanceApp.Models;

public class Role : IdentityRole<Guid>
{
    public Boolean IsActive  { get; set; }
}
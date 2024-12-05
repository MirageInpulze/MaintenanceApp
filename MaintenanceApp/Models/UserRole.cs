﻿using Microsoft.AspNetCore.Identity;

namespace MaintenanceApp.Models;

public class UserRole: IdentityUserRole<Guid>
{
    public Boolean IsActive  { get; set; }
}
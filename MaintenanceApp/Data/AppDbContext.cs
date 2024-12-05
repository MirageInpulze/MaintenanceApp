using MaintenanceApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MaintenanceApp.Data;

public class AppDbContext : IdentityDbContext<User, Role, Guid>
{   
    #region DbSet

    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<IncidentReport> IncidentReports { get; set; }
    public DbSet<ReportNotes> ReportNotes { get; set; }
    public DbSet<RoomStatus> RoomsStatus { get; set; }

    #endregion

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        #region Relations
        // Configure IncidentReport to ReportNotes relationship with ON DELETE Restrict
        modelBuilder.Entity<ReportNotes>()
            .HasOne(rn => rn.IncidentReport)
            .WithMany()
            .HasForeignKey(rn => rn.IncidentReportId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure IncidentReport to User relationship with ON DELETE Restrict
        modelBuilder.Entity<IncidentReport>()
            .HasOne(rn => rn.User)
            .WithMany()
            .HasForeignKey(rn => rn.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure RNotes to User relationship with ON DELETE Restrict
        modelBuilder.Entity<ReportNotes>()
            .HasOne(rn => rn.User)
            .WithMany()
            .HasForeignKey(rn => rn.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // Configure RoomStatus to IncidentReport relationship with ON DELETE Restrict
        modelBuilder.Entity<IncidentReport>()
            .HasOne(ir => ir.RoomStatus)
            .WithMany()
            .HasForeignKey(ir => ir.RoomStatusId)
            .OnDelete(DeleteBehavior.Restrict);
        #endregion
    }
}
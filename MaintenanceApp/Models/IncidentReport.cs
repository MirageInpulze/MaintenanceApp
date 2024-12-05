namespace MaintenanceApp.Models;

public class IncidentReport
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedLog { get; set; }
    public DateTime ModifiedDate { get; set; }  // MUSTHAVES
    public string ModifiedBy { get; set; }
    public string ModifiedLog { get; set; }
    public string IsActive  { get; set; }
    public Guid RoomStatusId { get; set; }
    public RoomStatus RoomStatus { get; set; }
    public string IncidentTypeDescription { get; set; } //Short Summary
    public string IncidentSeverity { get; set; } //Minor, Medium, Priority, Routine
    public string IncidentStatus { get; set; } //Todo, InProgress, Done, Reopened, Closed.
    public string? Description { get; set; } //Details
    public Guid UserId { get; set; }
    public User User { get; set; }
    public string? AssignedTo { get; set; } //Staff
    public string? AssignedBy { get; set; } //Supervisor
}
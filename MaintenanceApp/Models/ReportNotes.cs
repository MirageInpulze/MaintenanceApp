namespace MaintenanceApp.Models;

public class ReportNotes
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedLog { get; set; }
    public DateTime ModifiedDate { get; set; }  // MUSTHAVES
    public string ModifiedBy { get; set; }
    public string ModifiedLog { get; set; }
    public Boolean IsActive  { get; set; }
    public Guid IncidentReportId { get; set; }
    public IncidentReport? IncidentReport{ get; set; } //Match with IncidentReport ID
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
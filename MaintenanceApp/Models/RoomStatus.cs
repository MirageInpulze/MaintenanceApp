namespace MaintenanceApp.Models;

public class RoomStatus
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
    public string CreatedLog { get; set; }
    public DateTime ModifiedDate { get; set; }  // MUSTHAVES
    public string ModifiedBy { get; set; }
    public string ModifiedLog { get; set; }
    public string IsActive  { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Status { get; set; }
    
}
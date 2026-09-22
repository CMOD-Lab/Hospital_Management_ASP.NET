namespace ClinicManagementSystem.Domain.Entities;

public class Notification : BaseEntity
{
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
}

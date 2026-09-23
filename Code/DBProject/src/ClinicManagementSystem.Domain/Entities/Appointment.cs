namespace ClinicManagementSystem.Domain.Entities;

public sealed class Appointment : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Prescription { get; set; }
    public string? ProgressNotes { get; set; }
    public string? Disease { get; set; }
    public Bill? Bill { get; set; }
}

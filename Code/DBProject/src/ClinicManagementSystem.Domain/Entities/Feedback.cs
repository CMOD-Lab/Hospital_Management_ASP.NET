namespace ClinicManagementSystem.Domain.Entities;

public sealed class Feedback : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public int Rating { get; set; }
    public string Comments { get; set; } = string.Empty;
}

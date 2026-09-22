namespace ClinicManagementSystem.Domain.Entities;

public class Feedback : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public string Comments { get; set; } = string.Empty;
    public int Rating { get; set; }
}

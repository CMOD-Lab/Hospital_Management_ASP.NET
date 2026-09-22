namespace ClinicManagementSystem.Domain.Entities;

public class Bill : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime IssuedOn { get; set; } = DateTime.UtcNow;
}

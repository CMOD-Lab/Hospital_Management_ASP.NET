namespace ClinicManagementSystem.Domain.Entities;

public class Appointment : BaseEntity
{
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = "Pending";
    public string Notes { get; set; } = string.Empty;
    public TreatmentRecord? TreatmentRecord { get; set; }
    public Bill? Bill { get; set; }
    public Feedback? Feedback { get; set; }
}

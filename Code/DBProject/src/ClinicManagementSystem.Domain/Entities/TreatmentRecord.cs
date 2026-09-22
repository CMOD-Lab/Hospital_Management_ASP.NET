namespace ClinicManagementSystem.Domain.Entities;

public class TreatmentRecord : BaseEntity
{
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public int DoctorId { get; set; }
    public Doctor? Doctor { get; set; }
    public int PatientId { get; set; }
    public Patient? Patient { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string ProgressNotes { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

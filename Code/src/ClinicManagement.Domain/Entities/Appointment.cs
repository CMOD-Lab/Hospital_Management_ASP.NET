namespace ClinicManagement.Domain.Entities;

/// <summary>Represents an appointment between a patient and a doctor.</summary>
public class Appointment
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public int Status { get; set; } // 0=pending, 1=approved, 2=completed
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool IsPaid { get; set; }
    public bool FeedbackGiven { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

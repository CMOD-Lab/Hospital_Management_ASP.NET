namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a treatment history record for a patient.</summary>
public class TreatmentHistory
{
    public int HistoryId { get; set; }
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public DateTime TreatmentDate { get; set; }
}

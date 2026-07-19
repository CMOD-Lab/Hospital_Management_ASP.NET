using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Entities;

/// <summary>Appointment entity</summary>
public class Appointment
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlot { get; set; }
    public string Timings { get; set; } = string.Empty;
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool IsPaid { get; set; }
    public bool FeedbackGiven { get; set; }
    public DateTime AppointmentDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Doctor? Doctor { get; set; }
    public Patient? Patient { get; set; }
    public Bill? Bill { get; set; }
}

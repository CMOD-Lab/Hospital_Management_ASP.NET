namespace CareTrack.Application.DTOs;

/// <summary>
/// DTO for appointment information display.
/// </summary>
public class AppointmentDto
{
    public int AppointId { get; set; }
    public int? DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Timings { get; set; } = string.Empty;
    public int AppointmentStatus { get; set; }
    public string AppointmentStatusText { get; set; } = string.Empty;
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public int? FeedbackStatus { get; set; }
    public int? PatientNotification { get; set; }
}

/// <summary>
/// DTO for creating a new appointment.
/// </summary>
public class AppointmentCreateDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlotId { get; set; }
}

/// <summary>
/// DTO for updating appointment prescription.
/// </summary>
public class PrescriptionUpdateDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

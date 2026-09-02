using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for appointment data.
/// </summary>
public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string? DoctorName { get; set; }
    public string? PatientName { get; set; }
    public string? Timings { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public BillStatus BillStatus { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public int FeedbackStatus { get; set; }
}

/// <summary>
/// DTO for booking an appointment.
/// </summary>
public class AppointmentBookDto
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public int FreeSlot { get; set; }
}

/// <summary>
/// DTO for updating a prescription.
/// </summary>
public class PrescriptionUpdateDto
{
    public int DoctorId { get; set; }
    public int AppointmentId { get; set; }
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
}

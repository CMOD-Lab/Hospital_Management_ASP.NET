namespace ClinicManagement.Domain.Entities;

/// <summary>Bill entity</summary>
public class Bill
{
    public int BillId { get; set; }
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BillDate { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Appointment? Appointment { get; set; }
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a billing record for a patient appointment.</summary>
public class Bill
{
    public int BillId { get; set; }
    public int PatientId { get; set; }
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BillDate { get; set; } = DateTime.UtcNow;
}

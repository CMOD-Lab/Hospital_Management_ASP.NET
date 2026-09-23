namespace ClinicManagementSystem.Domain.Entities;

public sealed class Bill : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int AppointmentId { get; set; }
    public Appointment? Appointment { get; set; }
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
}

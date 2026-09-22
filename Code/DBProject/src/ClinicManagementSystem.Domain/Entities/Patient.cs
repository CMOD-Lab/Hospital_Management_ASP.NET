namespace ClinicManagementSystem.Domain.Entities;

public class Patient : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<TreatmentRecord> TreatmentRecords { get; set; } = new List<TreatmentRecord>();
    public ICollection<Bill> Bills { get; set; } = new List<Bill>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Feedback> FeedbackEntries { get; set; } = new List<Feedback>();
}

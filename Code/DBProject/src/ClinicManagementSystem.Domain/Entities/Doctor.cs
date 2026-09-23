namespace ClinicManagementSystem.Domain.Entities;

public sealed class Doctor : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public decimal ChargesPerVisit { get; set; }
    public int ExperienceYears { get; set; }
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }

    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

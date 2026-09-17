namespace ClinicManagement.Domain.Entities;

/// <summary>Represents a staff member in the clinic system.</summary>
public class Staff
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public int Status { get; set; } = 1;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

namespace ClinicManagement.Domain.Entities;

/// <summary>
/// Represents a non-doctor staff member.
/// </summary>
public class OtherStaff
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public char Gender { get; set; }
    public string? Designation { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
}

namespace CareTrack.Application.DTOs;

/// <summary>
/// DTO for staff information display.
/// </summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Designation { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public double? Salary { get; set; }
}

/// <summary>
/// DTO for creating a new staff member.
/// </summary>
public class StaffCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public double Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>
/// DTO for department information.
/// </summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DoctorCount { get; set; }
}

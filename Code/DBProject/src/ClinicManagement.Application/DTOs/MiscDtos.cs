namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for department data.
/// </summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// DTO for staff data.
/// </summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
}

/// <summary>
/// DTO for creating a new staff member.
/// </summary>
public class StaffCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
    public string? Designation { get; set; }
}

/// <summary>
/// DTO for admin home dashboard data.
/// </summary>
public class AdminHomeDto
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>
/// DTO for login result.
/// </summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public int UserType { get; set; }
    public string Message { get; set; } = string.Empty;
}

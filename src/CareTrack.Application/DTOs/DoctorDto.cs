namespace CareTrack.Application.DTOs;

/// <summary>
/// DTO for doctor information display.
/// </summary>
public class DoctorDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int? WorkExperience { get; set; }
    public int Age { get; set; }
    public int Status { get; set; }
}

/// <summary>
/// DTO for creating a new doctor.
/// </summary>
public class DoctorCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int WorkExperience { get; set; }
    public double MonthlySalary { get; set; }
    public double ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

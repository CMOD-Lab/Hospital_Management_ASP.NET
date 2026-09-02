namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for doctor data.
/// </summary>
public class DoctorDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public double ChargesPerVisit { get; set; }
    public int WorkExperience { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public int Status { get; set; }
    public int Age => CalculateAge(BirthDate);

    private static int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }
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
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int WorkExperience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
}

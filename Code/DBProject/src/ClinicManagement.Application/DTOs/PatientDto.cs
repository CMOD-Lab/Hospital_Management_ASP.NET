namespace ClinicManagement.Application.DTOs;

/// <summary>
/// DTO for patient data.
/// </summary>
public class PatientDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
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
/// DTO for creating a new patient.
/// </summary>
public class PatientCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

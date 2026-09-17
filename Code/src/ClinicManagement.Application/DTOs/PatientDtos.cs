namespace ClinicManagement.Application.DTOs;

/// <summary>DTO for login request.</summary>
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// <summary>DTO for patient registration.</summary>
public class PatientRegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

/// <summary>DTO for patient data display.</summary>
public class PatientDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

/// <summary>DTO for bill history display.</summary>
public class BillDto
{
    public int BillId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BillDate { get; set; }
}

/// <summary>DTO for treatment history display.</summary>
public class TreatmentHistoryDto
{
    public int HistoryId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public DateTime TreatmentDate { get; set; }
}

/// <summary>DTO for appointment display.</summary>
public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public int Status { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public bool IsPaid { get; set; }
    public bool FeedbackGiven { get; set; }
}

namespace ClinicManagement.Application.DTOs;

// ─── Auth DTOs ───────────────────────────────────────────────────────────────

/// <summary>Result of a login attempt.</summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public int UserId { get; set; }
    public int UserType { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>Patient sign-up input data.</summary>
public class PatientSignUpDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNo { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

/// <summary>Result of a sign-up attempt.</summary>
public class SignUpResultDto
{
    public bool Success { get; set; }
    public int PatientId { get; set; }
    public string? ErrorMessage { get; set; }
}

// ─── Admin DTOs ───────────────────────────────────────────────────────────────

/// <summary>Summary data for the admin home page.</summary>
public class AdminHomeDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentSummaryDto> Departments { get; set; } = new List<DepartmentSummaryDto>();
    public IEnumerable<AppointmentSummaryDto> Appointments { get; set; } = new List<AppointmentSummaryDto>();
}

public class DepartmentSummaryDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}

public class AppointmentSummaryDto
{
    public int AppointId { get; set; }
    public string? PatientName { get; set; }
    public string? DoctorName { get; set; }
    public DateTime? Date { get; set; }
    public string? Status { get; set; }
}

/// <summary>Input DTO for adding a new doctor.</summary>
public class AddDoctorDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

/// <summary>Input DTO for adding a new staff member.</summary>
public class AddStaffDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public char Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>Doctor list item for admin views.</summary>
public class DoctorListItemDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
}

/// <summary>Patient list item for admin views.</summary>
public class PatientListItemDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
}

/// <summary>Staff list item for admin views.</summary>
public class StaffListItemDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>Department DTO.</summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

// ─── Patient DTOs ─────────────────────────────────────────────────────────────

/// <summary>Patient personal information.</summary>
public class PatientInfoDto
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

/// <summary>Bill history entry for a patient.</summary>
public class BillHistoryDto
{
    public int AppointId { get; set; }
    public string? DoctorName { get; set; }
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>Current appointment for a patient.</summary>
public class CurrentAppointmentDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>Treatment history entry for a patient.</summary>
public class TreatmentHistoryDto
{
    public int AppointId { get; set; }
    public string? DoctorName { get; set; }
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>Doctor profile for patient view.</summary>
public class DoctorProfileDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public float ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string? Specialization { get; set; }
    public int WorkExperience { get; set; }
    public int Age { get; set; }
}

/// <summary>Free appointment slot.</summary>
public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string SlotTime { get; set; } = string.Empty;
}

/// <summary>Result of inserting an appointment.</summary>
public class AppointmentResultDto
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

/// <summary>Notification for a patient.</summary>
public class NotificationDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>Pending feedback for a patient.</summary>
public class PendingFeedbackDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

// ─── Doctor DTOs ──────────────────────────────────────────────────────────────

/// <summary>Doctor information for doctor home page.</summary>
public class DoctorInfoDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Department { get; set; }
    public string? Specialization { get; set; }
    public string? Qualification { get; set; }
    public double ChargesPerVisit { get; set; }
    public double? ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
}

/// <summary>Pending appointment for doctor view.</summary>
public class PendingAppointmentDto
{
    public int AppointId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Status { get; set; }
}

/// <summary>Today's appointment for doctor view.</summary>
public class TodayAppointmentDto
{
    public int AppointId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
}

/// <summary>Bill entry for doctor billing view.</summary>
public class BillDto
{
    public int AppointId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public double? BillAmount { get; set; }
    public string? BillStatus { get; set; }
}

/// <summary>Patient history entry for doctor view.</summary>
public class PatientHistoryDto
{
    public int AppointId { get; set; }
    public string? PatientName { get; set; }
    public DateTime? Date { get; set; }
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public string? BillStatus { get; set; }
}

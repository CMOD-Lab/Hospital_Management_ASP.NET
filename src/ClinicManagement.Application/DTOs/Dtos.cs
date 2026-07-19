namespace ClinicManagement.Application.DTOs;

// ─── Patient DTOs ───────────────────────────────────────────────────────────

/// <summary>Patient data transfer object</summary>
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

// ─── Doctor DTOs ────────────────────────────────────────────────────────────

/// <summary>Doctor data transfer object</summary>
public class DoctorDto
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public decimal ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Age { get; set; }
}

/// <summary>Doctor create DTO</summary>
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
    public int Experience { get; set; }
    public int Salary { get; set; }
    public int ChargesPerVisit { get; set; }
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
}

// ─── Appointment DTOs ────────────────────────────────────────────────────────

/// <summary>Appointment data transfer object</summary>
public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Disease { get; set; }
    public string? Prescription { get; set; }
    public DateTime AppointmentDate { get; set; }
}

/// <summary>Current appointment DTO</summary>
public class CurrentAppointmentDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

/// <summary>Free slot DTO</summary>
public class FreeSlotDto
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
}

/// <summary>Notification DTO</summary>
public class NotificationDto
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>Pending feedback DTO</summary>
public class PendingFeedbackDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
}

// ─── Bill DTOs ───────────────────────────────────────────────────────────────

/// <summary>Bill data transfer object</summary>
public class BillDto
{
    public int BillId { get; set; }
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public DateTime BillDate { get; set; }
}

/// <summary>Bill history DTO</summary>
public class BillHistoryDto
{
    public int Count { get; set; }
    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();
}

// ─── Treatment History DTOs ──────────────────────────────────────────────────

/// <summary>Treatment history DTO</summary>
public class TreatmentHistoryDto
{
    public int Count { get; set; }
    public IEnumerable<TreatmentRecordDto> Records { get; set; } = new List<TreatmentRecordDto>();
}

/// <summary>Treatment record DTO</summary>
public class TreatmentRecordDto
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
}

/// <summary>Patient history DTO (for doctor view)</summary>
public class PatientHistoryDto
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public DateTime AppointmentDate { get; set; }
}

// ─── Staff DTOs ──────────────────────────────────────────────────────────────

/// <summary>Staff data transfer object</summary>
public class StaffDto
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public int Salary { get; set; }
}

/// <summary>Staff create DTO</summary>
public class StaffCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Salary { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

// ─── Admin DTOs ──────────────────────────────────────────────────────────────

/// <summary>Admin dashboard DTO</summary>
public class AdminDashboardDto
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal TotalIncome { get; set; }
    public IEnumerable<DepartmentStatsDto> DepartmentStats { get; set; } = new List<DepartmentStatsDto>();
    public IEnumerable<AppointmentDto> RecentAppointments { get; set; } = new List<AppointmentDto>();
}

/// <summary>Department statistics DTO</summary>
public class DepartmentStatsDto
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
}

// ─── Department DTOs ─────────────────────────────────────────────────────────

/// <summary>Department DTO</summary>
public class DepartmentDto
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
}

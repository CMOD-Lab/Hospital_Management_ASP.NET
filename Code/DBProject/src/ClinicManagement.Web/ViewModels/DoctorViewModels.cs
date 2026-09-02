namespace ClinicManagement.Web.ViewModels;

/// <summary>
/// ViewModel for doctor home page.
/// </summary>
public class DoctorHomeViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public int WorkExperience { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
}

/// <summary>
/// ViewModel for pending appointments.
/// </summary>
public class PendingAppointmentViewModel
{
    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
    public string? Message { get; set; }
}

/// <summary>
/// ViewModel for patient history (today's appointments).
/// </summary>
public class PatientHistoryViewModel
{
    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
}

/// <summary>
/// ViewModel for history update (prescription).
/// </summary>
public class HistoryUpdateViewModel
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string? Message { get; set; }
}

/// <summary>
/// ViewModel for bill generation.
/// </summary>
public class BillViewModel
{
    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
    public string? Message { get; set; }
}

/// <summary>
/// ViewModel for previous history.
/// </summary>
public class PreviousHistoryViewModel
{
    public IEnumerable<AppointmentViewModel> History { get; set; } = new List<AppointmentViewModel>();
}

/// <summary>
/// Shared ViewModel for a doctor.
/// </summary>
public class DoctorViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int DeptNo { get; set; }
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public int WorkExperience { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public int Age { get; set; }
    public int Status { get; set; }
}

/// <summary>
/// Shared ViewModel for a department.
/// </summary>
public class DepartmentViewModel
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
    public string? Description { get; set; }
}

/// <summary>
/// Shared ViewModel for an appointment.
/// </summary>
public class AppointmentViewModel
{
    public int AppointmentId { get; set; }
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public string? DoctorName { get; set; }
    public string? PatientName { get; set; }
    public string? Timings { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Disease { get; set; }
    public string? Progress { get; set; }
    public string? Prescription { get; set; }
    public string BillStatus { get; set; } = string.Empty;
    public DateTime? AppointmentDate { get; set; }
    public int FeedbackStatus { get; set; }
}

/// <summary>
/// Shared ViewModel for a staff member.
/// </summary>
public class StaffViewModel
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Designation { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
}

using System.ComponentModel.DataAnnotations;

namespace ClinicManagement.Web.ViewModels;

// ─── Auth ViewModels ─────────────────────────────────────────────────────────

/// <summary>Login view model</summary>
public class LoginViewModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Sign up view model</summary>
public class SignUpViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(20, ErrorMessage = "Name cannot exceed 20 characters")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(30, ErrorMessage = "Email cannot exceed 30 characters")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 20 characters")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required")]
    [StringLength(15, ErrorMessage = "Phone cannot exceed 15 characters")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(40, ErrorMessage = "Address cannot exceed 40 characters")]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;
}

// ─── Patient ViewModels ──────────────────────────────────────────────────────

/// <summary>Patient home view model</summary>
public class PatientHomeViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Address { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
}

/// <summary>View doctors view model</summary>
public class ViewDoctorsViewModel
{
    public string SelectedDepartment { get; set; } = string.Empty;
    public IEnumerable<DepartmentItemViewModel> Departments { get; set; } = new List<DepartmentItemViewModel>();
    public IEnumerable<DoctorItemViewModel> Doctors { get; set; } = new List<DoctorItemViewModel>();
}

/// <summary>Department item view model</summary>
public class DepartmentItemViewModel
{
    public int DeptNo { get; set; }
    public string DeptName { get; set; } = string.Empty;
}

/// <summary>Doctor item view model</summary>
public class DoctorItemViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public decimal ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public string Specialization { get; set; } = string.Empty;
}

/// <summary>Doctor profile view model</summary>
public class DoctorProfileViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public decimal ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Age { get; set; }
}

/// <summary>Appointment taker view model</summary>
public class AppointmentTakerViewModel
{
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public IEnumerable<FreeSlotItemViewModel> FreeSlots { get; set; } = new List<FreeSlotItemViewModel>();
}

/// <summary>Free slot item view model</summary>
public class FreeSlotItemViewModel
{
    public int SlotId { get; set; }
    public string Timings { get; set; } = string.Empty;
}

/// <summary>Appointment request sent view model</summary>
public class AppointmentRequestSentViewModel
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}

/// <summary>Current appointment view model</summary>
public class CurrentAppointmentViewModel
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool HasAppointment { get; set; }
}

/// <summary>Bill history view model</summary>
public class BillHistoryViewModel
{
    public int Count { get; set; }
    public IEnumerable<BillItemViewModel> Bills { get; set; } = new List<BillItemViewModel>();
}

/// <summary>Bill item view model</summary>
public class BillItemViewModel
{
    public int BillId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsPaid { get; set; }
    public string BillDate { get; set; } = string.Empty;
}

/// <summary>Treatment history view model</summary>
public class TreatmentHistoryViewModel
{
    public int Count { get; set; }
    public IEnumerable<TreatmentRecordItemViewModel> Records { get; set; } = new List<TreatmentRecordItemViewModel>();
}

/// <summary>Treatment record item view model</summary>
public class TreatmentRecordItemViewModel
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public string AppointmentDate { get; set; } = string.Empty;
}

/// <summary>Patient notifications view model</summary>
public class PatientNotificationsViewModel
{
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool HasNotification { get; set; }
}

/// <summary>Patient feedback view model</summary>
public class PatientFeedbackViewModel
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public bool HasPendingFeedback { get; set; }
    public bool FeedbackSubmitted { get; set; }
}

// ─── Doctor ViewModels ───────────────────────────────────────────────────────

/// <summary>Doctor home view model</summary>
public class DoctorHomeViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public decimal ChargesPerVisit { get; set; }
    public float ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string Qualification { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int Experience { get; set; }
    public int Age { get; set; }
}

/// <summary>Pending appointment view model</summary>
public class PendingAppointmentViewModel
{
    public IEnumerable<AppointmentItemViewModel> Appointments { get; set; } = new List<AppointmentItemViewModel>();
}

/// <summary>Appointment item view model</summary>
public class AppointmentItemViewModel
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AppointmentDate { get; set; } = string.Empty;
}

/// <summary>History update view model</summary>
public class HistoryUpdateViewModel
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Disease is required")]
    [StringLength(30)]
    public string Disease { get; set; } = string.Empty;

    [Required(ErrorMessage = "Progress is required")]
    [StringLength(50)]
    public string Progress { get; set; } = string.Empty;

    [Required(ErrorMessage = "Prescription is required")]
    [StringLength(60)]
    public string Prescription { get; set; } = string.Empty;

    public IEnumerable<AppointmentItemViewModel> TodaysAppointments { get; set; } = new List<AppointmentItemViewModel>();
}

/// <summary>Bill view model (doctor)</summary>
public class DoctorBillViewModel
{
    public IEnumerable<BillItemViewModel> Bills { get; set; } = new List<BillItemViewModel>();
    public string Message { get; set; } = string.Empty;
}

/// <summary>Patient history view model (doctor)</summary>
public class PatientHistoryViewModel
{
    public IEnumerable<PatientHistoryItemViewModel> Records { get; set; } = new List<PatientHistoryItemViewModel>();
}

/// <summary>Patient history item view model</summary>
public class PatientHistoryItemViewModel
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string Disease { get; set; } = string.Empty;
    public string Prescription { get; set; } = string.Empty;
    public string Progress { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
    public string AppointmentDate { get; set; } = string.Empty;
}

// ─── Admin ViewModels ────────────────────────────────────────────────────────

/// <summary>Admin home view model</summary>
public class AdminHomeViewModel
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal TotalIncome { get; set; }
    public IEnumerable<DepartmentStatsItemViewModel> DepartmentStats { get; set; } = new List<DepartmentStatsItemViewModel>();
    public IEnumerable<AppointmentItemViewModel> RecentAppointments { get; set; } = new List<AppointmentItemViewModel>();
}

/// <summary>Department stats item view model</summary>
public class DepartmentStatsItemViewModel
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
    public int PatientCount { get; set; }
}

/// <summary>Manage clinic view model</summary>
public class ManageClinicViewModel
{
    public string Category { get; set; } = "DOCTOR";
    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<DoctorItemViewModel> Doctors { get; set; } = new List<DoctorItemViewModel>();
    public IEnumerable<PatientItemViewModel> Patients { get; set; } = new List<PatientItemViewModel>();
    public IEnumerable<StaffItemViewModel> Staff { get; set; } = new List<StaffItemViewModel>();
    public string Message { get; set; } = string.Empty;
    public string? SelectedDetails { get; set; }
}

/// <summary>Patient item view model</summary>
public class PatientItemViewModel
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

/// <summary>Staff item view model</summary>
public class StaffItemViewModel
{
    public int StaffId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
}

/// <summary>Doctor registration view model</summary>
public class DoctorRegistrationViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress]
    [StringLength(30)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(30, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Department is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Please select a department")]
    public int DeptNo { get; set; }

    [Required(ErrorMessage = "Phone is required")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(30)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Experience is required")]
    [Range(0, 50)]
    public int Experience { get; set; }

    [Required(ErrorMessage = "Salary is required")]
    [Range(0, int.MaxValue)]
    public int Salary { get; set; }

    [Required(ErrorMessage = "Charges per visit is required")]
    [Range(0, int.MaxValue)]
    public int ChargesPerVisit { get; set; }

    [Required(ErrorMessage = "Specialization is required")]
    [StringLength(50)]
    public string Specialization { get; set; } = string.Empty;

    [Required(ErrorMessage = "Qualification is required")]
    [StringLength(100)]
    public string Qualification { get; set; } = string.Empty;

    public IEnumerable<DepartmentItemViewModel> Departments { get; set; } = new List<DepartmentItemViewModel>();
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>Add staff view model</summary>
public class AddStaffViewModel
{
    [Required(ErrorMessage = "Name is required")]
    [StringLength(30)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required")]
    public string BirthDate { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone is required")]
    [StringLength(30)]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required")]
    public string Gender { get; set; } = string.Empty;

    [Required(ErrorMessage = "Address is required")]
    [StringLength(50)]
    public string Address { get; set; } = string.Empty;

    [Required(ErrorMessage = "Salary is required")]
    [Range(0, int.MaxValue)]
    public int Salary { get; set; }

    [Required(ErrorMessage = "Qualification is required")]
    [StringLength(1)]
    public string Qualification { get; set; } = string.Empty;

    [Required(ErrorMessage = "Designation is required")]
    [StringLength(30)]
    public string Designation { get; set; } = string.Empty;

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

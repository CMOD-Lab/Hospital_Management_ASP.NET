namespace ClinicManagement.Web.ViewModels;

/// <summary>
/// ViewModel for patient home page.
/// </summary>
public class PatientHomeViewModel
{
    public int PatientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string BirthDate { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for viewing doctors by department.
/// </summary>
public class ViewDoctorsViewModel
{
    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    public string SelectedDepartment { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel for doctor profile.
/// </summary>
public class DoctorProfileViewModel
{
    public int DoctorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public double ChargesPerVisit { get; set; }
    public double ReputeIndex { get; set; }
    public int PatientsTreated { get; set; }
    public string? Qualification { get; set; }
    public string? Specialization { get; set; }
    public int WorkExperience { get; set; }
    public int Age { get; set; }
}

/// <summary>
/// ViewModel for taking an appointment.
/// </summary>
public class TakeAppointmentViewModel
{
    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public string SelectedDepartment { get; set; } = string.Empty;
    public int SelectedDoctorId { get; set; }
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
}

/// <summary>
/// ViewModel for appointment taker (slot selection).
/// </summary>
public class AppointmentTakerViewModel
{
    public int DoctorId { get; set; }
    public int PatientId { get; set; }
    public IEnumerable<AppointmentViewModel> FreeSlots { get; set; } = new List<AppointmentViewModel>();
    public string? Message { get; set; }
}

/// <summary>
/// ViewModel for current appointment.
/// </summary>
public class CurrentAppointmentViewModel
{
    public string? DoctorName { get; set; }
    public string? Timings { get; set; }
    public bool HasAppointment { get; set; }
}

/// <summary>
/// ViewModel for bills history.
/// </summary>
public class BillsHistoryViewModel
{
    public IEnumerable<AppointmentViewModel> Bills { get; set; } = new List<AppointmentViewModel>();
}

/// <summary>
/// ViewModel for treatment history.
/// </summary>
public class TreatmentHistoryViewModel
{
    public IEnumerable<AppointmentViewModel> Treatments { get; set; } = new List<AppointmentViewModel>();
}

/// <summary>
/// ViewModel for patient notifications.
/// </summary>
public class PatientNotificationsViewModel
{
    public string? DoctorName { get; set; }
    public string? Timings { get; set; }
    public bool HasNotification { get; set; }
}

/// <summary>
/// ViewModel for patient feedback.
/// </summary>
public class PatientFeedbackViewModel
{
    public int AppointmentId { get; set; }
    public string? DoctorName { get; set; }
    public string? Timings { get; set; }
    public bool HasPendingFeedback { get; set; }
    public string? Message { get; set; }
}

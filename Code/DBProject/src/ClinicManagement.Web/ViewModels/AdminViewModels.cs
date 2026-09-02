namespace ClinicManagement.Web.ViewModels;

/// <summary>
/// ViewModel for the admin home dashboard.
/// </summary>
public class AdminHomeViewModel
{
    public int TotalPatients { get; set; }
    public int TotalDoctors { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public IEnumerable<AppointmentViewModel> RecentAppointments { get; set; } = new List<AppointmentViewModel>();
}

/// <summary>
/// ViewModel for managing clinic (doctors and staff).
/// </summary>
public class ManageClinicViewModel
{
    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    public IEnumerable<StaffViewModel> Staff { get; set; } = new List<StaffViewModel>();
    public string? Message { get; set; }
}

/// <summary>
/// ViewModel for doctor registration form.
/// </summary>
public class DoctorRegistrationViewModel
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
    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

/// <summary>
/// ViewModel for adding staff.
/// </summary>
public class AddStaffViewModel
{
    public string Name { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int Salary { get; set; }
    public string? Qualification { get; set; }
    public string? Designation { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}

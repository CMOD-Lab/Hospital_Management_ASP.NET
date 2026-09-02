using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for the admin home dashboard.
/// </summary>
public class AdminHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly IDoctorService _doctorService;
    private readonly IAppointmentService _appointmentService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeModel(
        IPatientService patientService,
        IDoctorService doctorService,
        IAppointmentService appointmentService,
        IDepartmentService departmentService,
        ILogger<AdminHomeModel> logger)
    {
        _patientService = patientService;
        _doctorService = doctorService;
        _appointmentService = appointmentService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public AdminHomeViewModel DashboardData { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check admin session
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            var patients = await _patientService.GetAllPatientsAsync();
            var doctors = await _doctorService.GetAllDoctorsAsync();
            var departments = await _departmentService.GetAllDepartmentsAsync();
            var appointments = await _appointmentService.GetAllAsync();

            DashboardData = new AdminHomeViewModel
            {
                TotalPatients = patients.Count(),
                TotalDoctors = doctors.Count(),
                TotalIncome = appointments
                    .Where(a => a.BillStatus == Domain.Enums.BillStatus.Paid)
                    .Sum(a => a.Doctor?.ChargesPerVisit ?? 0),
                Departments = departments.Select(d => new DepartmentViewModel
                {
                    DeptNo = d.DeptNo,
                    DeptName = d.DeptName,
                    Description = d.Description
                }),
                RecentAppointments = appointments.Take(10).Select(a => new AppointmentViewModel
                {
                    AppointmentId = a.AppointmentId,
                    DoctorName = a.Doctor?.Name,
                    PatientName = a.Patient?.Name,
                    Status = a.Status.ToString(),
                    BillStatus = a.BillStatus.ToString()
                })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }

        return Page();
    }
}

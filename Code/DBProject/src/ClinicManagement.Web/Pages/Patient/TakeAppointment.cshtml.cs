using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for taking an appointment.
/// </summary>
public class TakeAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public TakeAppointmentModel(
        IDoctorService doctorService,
        IDepartmentService departmentService,
        IAppointmentService appointmentService,
        ILogger<TakeAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    public string SelectedDepartment { get; set; } = string.Empty;
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(string? deptName = null)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/Account/Login");

        SelectedDepartment = deptName ?? string.Empty;

        try
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments = departments.Select(d => new DepartmentViewModel
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName
            });

            if (!string.IsNullOrEmpty(SelectedDepartment))
            {
                var doctors = await _doctorService.GetDoctorsByDepartmentAsync(SelectedDepartment);
                Doctors = doctors.Select(d => new DoctorViewModel
                {
                    DoctorId = d.DoctorId,
                    Name = d.Name,
                    ChargesPerVisit = d.ChargesPerVisit
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading take appointment page");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var success = await _appointmentService.BookAppointmentAsync(doctorId, userId.Value, 0);
            if (success)
            {
                Message = "Appointment request sent successfully! Please wait for doctor approval.";
            }
            else
            {
                Message = "Failed to book appointment. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment");
            Message = "An error occurred while booking the appointment.";
        }

        return await OnGetAsync();
    }
}

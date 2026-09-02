using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for current appointment.
/// </summary>
public class CurrentAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public CurrentAppointmentModel(IAppointmentService appointmentService, ILogger<CurrentAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public CurrentAppointmentViewModel AppointmentInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var appointment = await _appointmentService.GetCurrentAppointmentByPatientAsync(userId.Value);
            AppointmentInfo = new CurrentAppointmentViewModel
            {
                HasAppointment = appointment != null,
                DoctorName = appointment?.Doctor?.Name,
                Timings = appointment?.Timings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment");
        }

        return Page();
    }
}

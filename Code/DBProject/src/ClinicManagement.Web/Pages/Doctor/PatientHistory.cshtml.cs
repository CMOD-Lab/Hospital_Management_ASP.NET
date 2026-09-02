using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for today's patient history.
/// </summary>
public class PatientHistoryModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PatientHistoryModel> _logger;

    public PatientHistoryModel(IAppointmentService appointmentService, ILogger<PatientHistoryModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var appointments = await _appointmentService.GetTodaysAppointmentsByDoctorAsync(userId.Value);
            Appointments = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.Patient?.Name,
                Timings = a.Timings,
                Status = a.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading today's patients");
        }

        return Page();
    }
}

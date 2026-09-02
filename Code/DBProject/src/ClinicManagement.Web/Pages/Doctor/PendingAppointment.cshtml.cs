using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for pending appointments.
/// </summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public PendingAppointmentModel(IAppointmentService appointmentService, ILogger<PendingAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public IEnumerable<AppointmentViewModel> Appointments { get; set; } = new List<AppointmentViewModel>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var appointments = await _appointmentService.GetPendingAppointmentsByDoctorAsync(userId.Value);
            Appointments = appointments.Select(a => new AppointmentViewModel
            {
                AppointmentId = a.AppointmentId,
                PatientName = a.Patient?.Name,
                AppointmentDate = a.AppointmentDate,
                Timings = a.Timings,
                Status = a.Status.ToString()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments");
            Message = "Error loading appointments.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 2) return RedirectToPage("/Account/Login");

        var success = await _appointmentService.ApproveAppointmentAsync(appointmentId);
        Message = success ? "Appointment approved." : "Error approving appointment.";
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 2) return RedirectToPage("/Account/Login");

        var success = await _appointmentService.DeleteAppointmentAsync(appointmentId);
        Message = success ? "Appointment rejected." : "Error rejecting appointment.";
        return await OnGetAsync();
    }
}

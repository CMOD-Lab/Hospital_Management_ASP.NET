using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Doctor;

/// <summary>
/// Page model for pending appointments management.
/// </summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public IEnumerable<Appointment> PendingAppointments { get; set; } = new List<Appointment>();
    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor ID: {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _doctorService.ApproveAppointmentAsync(appointmentId, cancellationToken);
        StatusMessage = success ? "Appointment approved successfully." : "Error approving appointment.";
        IsSuccess = success;

        PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 2)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _doctorService.RejectAppointmentAsync(appointmentId, cancellationToken);
        StatusMessage = success ? "Appointment rejected." : "Error rejecting appointment.";
        IsSuccess = success;

        PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}

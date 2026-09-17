using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Pending appointments page model (migrated from PendingAppointment.aspx.cs).</summary>
public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<PendingAppointmentModel> _logger;

    public string Message { get; set; } = string.Empty;
    public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

    public PendingAppointmentModel(IDoctorService doctorService, ILogger<PendingAppointmentModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Index");

        try
        {
            Appointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending appointments for doctor {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        await _doctorService.ApproveAppointmentAsync(appointmentId);
        Message = "Appointment approved successfully.";
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        await _doctorService.DeleteAppointmentAsync(appointmentId);
        Message = "Appointment rejected.";
        return await OnGetAsync();
    }
}

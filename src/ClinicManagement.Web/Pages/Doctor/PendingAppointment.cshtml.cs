using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PendingAppointmentModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public PendingAppointmentModel(IDoctorService doctorService) => _doctorService = doctorService;
    public IEnumerable<PendingAppointmentDto> PendingAppointments { get; set; } = new List<PendingAppointmentDto>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostApproveAsync(int appointmentId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        await _doctorService.ApproveAppointmentAsync(appointmentId, cancellationToken);
        Message = "Appointment approved.";
        PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        await _doctorService.DeleteAppointmentAsync(appointmentId, cancellationToken);
        Message = "Appointment rejected.";
        PendingAppointments = await _doctorService.GetPendingAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}

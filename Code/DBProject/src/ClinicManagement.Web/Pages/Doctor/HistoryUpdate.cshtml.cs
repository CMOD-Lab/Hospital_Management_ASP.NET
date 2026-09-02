using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for updating patient history/prescription.
/// </summary>
public class HistoryUpdateModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public HistoryUpdateModel(IAppointmentService appointmentService, ILogger<HistoryUpdateModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    [BindProperty]
    public HistoryUpdateViewModel Input { get; set; } = new();

    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null) return NotFound();

            Input = new HistoryUpdateViewModel
            {
                AppointmentId = appointment.AppointmentId,
                DoctorId = userId.Value,
                PatientName = appointment.Patient?.Name ?? string.Empty,
                Disease = appointment.Disease ?? string.Empty,
                Progress = appointment.Progress ?? string.Empty,
                Prescription = appointment.Prescription ?? string.Empty
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment for update");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var success = await _appointmentService.UpdatePrescriptionAsync(
                userId.Value,
                Input.AppointmentId,
                Input.Disease,
                Input.Progress,
                Input.Prescription);

            Message = success ? "History updated successfully." : "Error updating history.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription");
            Message = "An error occurred.";
        }

        return Page();
    }
}

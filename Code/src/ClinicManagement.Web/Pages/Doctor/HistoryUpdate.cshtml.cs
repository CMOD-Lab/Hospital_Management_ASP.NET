using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>History update page model (migrated from HistoryUpdate.aspx.cs).</summary>
public class HistoryUpdateModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<HistoryUpdateModel> _logger;

    public int AppointmentId { get; set; }
    public string Message { get; set; } = string.Empty;

    public HistoryUpdateModel(IDoctorService doctorService, ILogger<HistoryUpdateModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public IActionResult OnGet(int? appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Index");

        AppointmentId = appointmentId ?? 0;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, string disease, string progress, string prescription)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        AppointmentId = appointmentId;

        try
        {
            bool success = await _doctorService.UpdatePrescriptionAsync(
                userId.Value, appointmentId, disease, progress, prescription);

            Message = success ? "Patient history updated successfully." : "Failed to update history.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment {AppointmentId}", appointmentId);
            Message = "An error occurred while updating history.";
        }

        return Page();
    }
}

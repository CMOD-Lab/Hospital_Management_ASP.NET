using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient notifications page model (migrated from PatientNotifications.aspx.cs).</summary>
public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public IEnumerable<Appointment> Notifications { get; set; } = new List<Appointment>();

    public PatientNotificationsModel(IPatientService patientService, ILogger<PatientNotificationsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        try
        {
            Notifications = await _patientService.GetNotificationsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for patient {PatientId}", userId);
        }

        return Page();
    }
}

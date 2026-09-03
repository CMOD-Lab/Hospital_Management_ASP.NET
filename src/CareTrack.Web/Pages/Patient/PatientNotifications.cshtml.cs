using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for patient notifications.
/// </summary>
public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public Appointment? Notification { get; set; }

    public PatientNotificationsModel(IPatientService patientService, ILogger<PatientNotificationsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            Notification = await _patientService.GetNotificationAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications for patient ID: {PatientId}", userId);
        }

        return Page();
    }
}

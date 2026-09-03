using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for patient feedback.
/// </summary>
public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public Appointment? PendingFeedback { get; set; }
    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public PatientFeedbackModel(IPatientService patientService, ILogger<PatientFeedbackModel> logger)
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
            PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending feedback for patient ID: {PatientId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _patientService.SubmitFeedbackAsync(appointmentId, cancellationToken);
        StatusMessage = success ? "Thank you for your feedback!" : "No pending feedback found.";
        IsSuccess = success;

        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        return Page();
    }
}

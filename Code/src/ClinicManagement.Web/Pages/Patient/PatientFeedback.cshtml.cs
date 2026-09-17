using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient feedback page model (migrated from PatientFeedback.aspx.cs).</summary>
public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public bool HasPendingFeedback { get; set; }
    public Appointment? PendingAppointment { get; set; }
    public string Message { get; set; } = string.Empty;

    public PatientFeedbackModel(IPatientService patientService, ILogger<PatientFeedbackModel> logger)
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
            var (hasPending, appointment) = await _patientService.GetPendingFeedbackAsync(userId.Value);
            HasPendingFeedback = hasPending;
            PendingAppointment = appointment;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback for patient {PatientId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        try
        {
            bool success = await _patientService.SubmitFeedbackAsync(appointmentId);
            Message = success ? "Feedback submitted successfully. Thank you!" : "Failed to submit feedback.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment {AppointmentId}", appointmentId);
            Message = "An error occurred while submitting feedback.";
        }

        return Page();
    }
}

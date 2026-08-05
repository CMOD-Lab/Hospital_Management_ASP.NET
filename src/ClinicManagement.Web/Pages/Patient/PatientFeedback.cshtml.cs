using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    public PatientFeedbackModel(IPatientService patientService) => _patientService = patientService;
    public PendingFeedbackDto? PendingFeedback { get; set; }
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");
        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");

        bool success = await _patientService.SubmitFeedbackAsync(appointmentId, cancellationToken);
        Message = success ? "Feedback submitted successfully!" : "Failed to submit feedback.";
        IsSuccess = success;
        PendingFeedback = await _patientService.GetPendingFeedbackAsync(userId.Value, cancellationToken);
        return Page();
    }
}

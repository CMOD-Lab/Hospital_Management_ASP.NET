using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientFeedbackModel> _logger;
    public PatientFeedbackViewModel FeedbackInfo { get; set; } = new();

    public PatientFeedbackModel(IPatientService patientService, ILogger<PatientFeedbackModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");
            var feedback = await _patientService.GetPendingFeedbackAsync(patientId.Value, cancellationToken);
            if (feedback != null)
            {
                FeedbackInfo = new PatientFeedbackViewModel
                {
                    AppointmentId = feedback.AppointmentId,
                    DoctorName = feedback.DoctorName,
                    Timings = feedback.Timings,
                    HasPendingFeedback = true
                };
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback");
            return Page();
        }
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            await _patientService.SubmitFeedbackAsync(appointmentId, cancellationToken);
            FeedbackInfo.FeedbackSubmitted = true;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback");
            return Page();
        }
    }
}

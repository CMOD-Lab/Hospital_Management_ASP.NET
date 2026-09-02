using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientFeedbackModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PatientFeedbackModel> _logger;

    public PatientFeedbackModel(IAppointmentService appointmentService, ILogger<PatientFeedbackModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public PatientFeedbackViewModel FeedbackInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var feedback = await _appointmentService.GetPendingFeedbackByPatientAsync(userId.Value);
            FeedbackInfo = new PatientFeedbackViewModel
            {
                HasPendingFeedback = feedback != null,
                AppointmentId = feedback?.AppointmentId ?? 0,
                DoctorName = feedback?.Doctor?.Name,
                Timings = feedback?.Timings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading feedback");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int appointmentId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/Account/Login");

        try
        {
            var success = await _appointmentService.StoreFeedbackAsync(appointmentId);
            FeedbackInfo = new PatientFeedbackViewModel
            {
                Message = success ? "Thank you for your feedback!" : "Error submitting feedback."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback");
            FeedbackInfo = new PatientFeedbackViewModel { Message = "An error occurred." };
        }

        return Page();
    }
}

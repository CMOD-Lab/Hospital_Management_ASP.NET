using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientNotificationsModel> _logger;
    public PatientNotificationsViewModel NotificationInfo { get; set; } = new();

    public PatientNotificationsModel(IPatientService patientService, ILogger<PatientNotificationsModel> logger)
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
            var notif = await _patientService.GetNotificationsAsync(patientId.Value, cancellationToken);
            if (notif != null)
            {
                NotificationInfo = new PatientNotificationsViewModel
                {
                    DoctorName = notif.DoctorName,
                    Timings = notif.Timings,
                    HasNotification = true
                };
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications");
            return Page();
        }
    }
}

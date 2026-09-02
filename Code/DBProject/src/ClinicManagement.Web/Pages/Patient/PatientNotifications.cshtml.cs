using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<PatientNotificationsModel> _logger;

    public PatientNotificationsModel(IAppointmentService appointmentService, ILogger<PatientNotificationsModel> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    public PatientNotificationsViewModel NotificationInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || !userId.HasValue) return RedirectToPage("/Account/Login");

        try
        {
            var notification = await _appointmentService.GetNotificationByPatientAsync(userId.Value);
            NotificationInfo = new PatientNotificationsViewModel
            {
                HasNotification = notification != null,
                DoctorName = notification?.Doctor?.Name,
                Timings = notification?.Timings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading notifications");
        }

        return Page();
    }
}

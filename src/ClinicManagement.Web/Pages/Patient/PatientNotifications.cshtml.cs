using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class PatientNotificationsModel : PageModel
{
    private readonly IPatientService _patientService;
    public PatientNotificationsModel(IPatientService patientService) => _patientService = patientService;
    public NotificationDto? Notification { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");
        Notification = await _patientService.GetNotificationsAsync(userId.Value, cancellationToken);
        return Page();
    }
}

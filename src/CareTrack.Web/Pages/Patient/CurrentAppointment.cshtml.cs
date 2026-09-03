using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for viewing current appointment.
/// </summary>
public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public Appointment? CurrentAppointment { get; set; }

    public CurrentAppointmentModel(IPatientService patientService, ILogger<CurrentAppointmentModel> logger)
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
            CurrentAppointment = await _patientService.GetCurrentAppointmentAsync(userId.Value, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment for patient ID: {PatientId}", userId);
        }

        return Page();
    }
}

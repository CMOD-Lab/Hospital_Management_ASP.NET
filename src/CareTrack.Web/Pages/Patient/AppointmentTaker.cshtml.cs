using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Patient;

/// <summary>
/// Page model for booking an appointment.
/// </summary>
public class AppointmentTakerModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<AppointmentTakerModel> _logger;

    public Domain.Entities.Doctor? Doctor { get; set; }
    public int DoctorId { get; set; }
    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public AppointmentTakerModel(IPatientService patientService, ILogger<AppointmentTakerModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        DoctorId = doctorId;

        try
        {
            Doctor = await _patientService.GetDoctorProfileAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment taker for doctor ID: {DoctorId}", doctorId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 1)
        {
            return RedirectToPage("/SignUp");
        }

        DoctorId = doctorId;
        Doctor = await _patientService.GetDoctorProfileAsync(doctorId, cancellationToken);

        var (success, message) = await _patientService.BookAppointmentAsync(
            doctorId, userId.Value, 0, cancellationToken);

        StatusMessage = message;
        IsSuccess = success;

        if (success)
        {
            return RedirectToPage("/Patient/AppointmentRequestSent");
        }

        return Page();
    }
}

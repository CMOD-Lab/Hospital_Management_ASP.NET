using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Current appointment page model (migrated from CurrentAppointment.aspx.cs).</summary>
public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<CurrentAppointmentModel> _logger;

    public bool HasAppointment { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Timings { get; set; } = string.Empty;

    public CurrentAppointmentModel(IPatientService patientService, ILogger<CurrentAppointmentModel> logger)
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
            var appointment = await _patientService.GetCurrentAppointmentAsync(userId.Value);
            if (appointment != null)
            {
                HasAppointment = true;
                DoctorName = appointment.DoctorName;
                Timings = appointment.Timings;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment for patient {PatientId}", userId);
        }

        return Page();
    }
}

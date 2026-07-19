using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<CurrentAppointmentModel> _logger;
    public CurrentAppointmentViewModel AppointmentInfo { get; set; } = new();

    public CurrentAppointmentModel(IPatientService patientService, ILogger<CurrentAppointmentModel> logger)
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
            var appt = await _patientService.GetCurrentAppointmentAsync(patientId.Value, cancellationToken);
            if (appt != null)
            {
                AppointmentInfo = new CurrentAppointmentViewModel
                {
                    DoctorName = appt.DoctorName,
                    Timings = appt.Timings,
                    HasAppointment = true
                };
            }
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading current appointment");
            return Page();
        }
    }
}

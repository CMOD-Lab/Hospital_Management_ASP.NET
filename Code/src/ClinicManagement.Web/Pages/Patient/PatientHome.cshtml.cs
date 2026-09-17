using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Patient home page model (migrated from PatientHome.aspx.cs).</summary>
public class PatientHomeModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<PatientHomeModel> _logger;

    public string PatientName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string BirthDate { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public bool HasCurrentAppointment { get; set; }
    public string CurrentDoctorName { get; set; } = string.Empty;
    public string CurrentTimings { get; set; } = string.Empty;

    public PatientHomeModel(IPatientService patientService, ILogger<PatientHomeModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 1)
            return RedirectToPage("/Index");

        try
        {
            var patient = await _patientService.GetPatientByIdAsync(userId.Value);
            if (patient != null)
            {
                PatientName = patient.Name;
                Phone = patient.Phone;
                Address = patient.Address;
                BirthDate = patient.BirthDate;
                Gender = patient.Gender;
            }

            var currentAppt = await _patientService.GetCurrentAppointmentAsync(userId.Value);
            if (currentAppt != null)
            {
                HasCurrentAppointment = true;
                CurrentDoctorName = currentAppt.DoctorName;
                CurrentTimings = currentAppt.Timings;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient home for patient {PatientId}", userId);
        }

        return Page();
    }
}

using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class DoctorProfileModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(IPatientService patientService, ILogger<DoctorProfileModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public DoctorProfileDto? DoctorProfile { get; set; }

    public async Task<IActionResult> OnGetAsync(int doctorId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/SignUp");

        DoctorProfile = await _patientService.GetDoctorProfileAsync(doctorId, cancellationToken);
        return Page();
    }
}

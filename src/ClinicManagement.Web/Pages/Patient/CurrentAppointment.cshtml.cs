using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class CurrentAppointmentModel : PageModel
{
    private readonly IPatientService _patientService;
    public CurrentAppointmentModel(IPatientService patientService) => _patientService = patientService;
    public CurrentAppointmentDto? CurrentAppointment { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");
        CurrentAppointment = await _patientService.GetCurrentAppointmentAsync(userId.Value, cancellationToken);
        return Page();
    }
}

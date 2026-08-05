using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class TreatmentHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    public TreatmentHistoryModel(IPatientService patientService) => _patientService = patientService;
    public IEnumerable<TreatmentHistoryDto> TreatmentHistory { get; set; } = new List<TreatmentHistoryDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");
        TreatmentHistory = await _patientService.GetTreatmentHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}

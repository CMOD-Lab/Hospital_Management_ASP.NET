using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    public BillsHistoryModel(IPatientService patientService) => _patientService = patientService;
    public IEnumerable<BillHistoryDto> Bills { get; set; } = new List<BillHistoryDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 1 || userId == null) return RedirectToPage("/SignUp");
        Bills = await _patientService.GetBillHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}

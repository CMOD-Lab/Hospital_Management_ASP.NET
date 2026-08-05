using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PreviousHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public PreviousHistoryModel(IDoctorService doctorService) => _doctorService = doctorService;
    public IEnumerable<PatientHistoryDto> PatientHistory { get; set; } = new List<PatientHistoryDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        PatientHistory = await _doctorService.GetPatientHistoryAsync(userId.Value, cancellationToken);
        return Page();
    }
}

using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public DoctorHomeModel(IDoctorService doctorService) => _doctorService = doctorService;
    public DoctorInfoDto? DoctorInfo { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        DoctorInfo = await _doctorService.GetDoctorInfoAsync(userId.Value, cancellationToken);
        return Page();
    }
}

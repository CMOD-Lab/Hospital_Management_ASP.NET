using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class PatientHistoryModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public PatientHistoryModel(IDoctorService doctorService) => _doctorService = doctorService;
    public IEnumerable<TodayAppointmentDto> TodaysAppointments { get; set; } = new List<TodayAppointmentDto>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        TodaysAppointments = await _doctorService.GetTodaysAppointmentsAsync(userId.Value, cancellationToken);
        return Page();
    }
}

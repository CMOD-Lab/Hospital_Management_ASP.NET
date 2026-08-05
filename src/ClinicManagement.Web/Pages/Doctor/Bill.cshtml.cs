using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    public BillModel(IDoctorService doctorService) => _doctorService = doctorService;
    public IEnumerable<BillDto> Bills { get; set; } = new List<BillDto>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        Bills = await _doctorService.GetBillsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId, cancellationToken);
        Message = "Bill marked as paid.";
        Bills = await _doctorService.GetBillsAsync(userId.Value, cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostMarkUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userType != 2 || userId == null) return RedirectToPage("/SignUp");
        await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId, cancellationToken);
        Message = "Bill marked as unpaid.";
        Bills = await _doctorService.GetBillsAsync(userId.Value, cancellationToken);
        return Page();
    }
}

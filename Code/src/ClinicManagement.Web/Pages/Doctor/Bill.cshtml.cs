using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Bill generation page model (migrated from Bill.aspx.cs).</summary>
public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public string Message { get; set; } = string.Empty;
    public IEnumerable<Bill> Bills { get; set; } = new List<Bill>();

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null || HttpContext.Session.GetInt32("UserType") != 2)
            return RedirectToPage("/Index");

        try
        {
            Bills = await _doctorService.GenerateBillsAsync(userId.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bills for doctor {DoctorId}", userId);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostPaidAsync(int appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        await _doctorService.MarkBillPaidAsync(userId.Value, appointmentId);
        Message = "Bill marked as paid.";
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostUnpaidAsync(int appointmentId)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null)
            return RedirectToPage("/Index");

        await _doctorService.MarkBillUnpaidAsync(userId.Value, appointmentId);
        Message = "Bill marked as unpaid.";
        return await OnGetAsync();
    }
}

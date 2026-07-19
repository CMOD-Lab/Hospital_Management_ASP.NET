using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Bill page model</summary>
public class BillModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<BillModel> _logger;

    public DoctorBillViewModel BillViewModel { get; set; } = new();

    public BillModel(IDoctorService doctorService, ILogger<BillModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadBillsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostPaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return RedirectToPage("/Index");

            await _doctorService.MarkBillPaidAsync(doctorId.Value, appointmentId, cancellationToken);
            BillViewModel.Message = "Bill marked as paid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid");
            BillViewModel.Message = "There was an error.";
        }
        await LoadBillsAsync(cancellationToken);
        return RedirectToPage("/Doctor/PatientHistory");
    }

    public async Task<IActionResult> OnPostUnpaidAsync(int appointmentId, CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return RedirectToPage("/Index");

            await _doctorService.MarkBillUnpaidAsync(doctorId.Value, appointmentId, cancellationToken);
            BillViewModel.Message = "Bill marked as unpaid.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid");
            BillViewModel.Message = "There was an error.";
        }
        await LoadBillsAsync(cancellationToken);
        return RedirectToPage("/Doctor/PatientHistory");
    }

    private async Task LoadBillsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var doctorId = HttpContext.Session.GetInt32("idoriginal");
            if (doctorId == null) return;

            var bills = await _doctorService.GenerateBillsAsync(doctorId.Value, cancellationToken);
            BillViewModel.Bills = bills.Select(b => new BillItemViewModel
            {
                BillId = b.BillId,
                PatientName = b.PatientName,
                Amount = b.Amount,
                IsPaid = b.IsPaid,
                BillDate = b.BillDate.ToString("yyyy-MM-dd")
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bills");
        }
    }
}

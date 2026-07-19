using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class BillsHistoryModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<BillsHistoryModel> _logger;
    public BillHistoryViewModel BillHistory { get; set; } = new();

    public BillsHistoryModel(IPatientService patientService, ILogger<BillsHistoryModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var patientId = HttpContext.Session.GetInt32("idoriginal");
            if (patientId == null) return RedirectToPage("/Index");
            var data = await _patientService.GetBillHistoryAsync(patientId.Value, cancellationToken);
            BillHistory = new BillHistoryViewModel
            {
                Count = data.Count,
                Bills = data.Bills.Select(b => new BillItemViewModel
                {
                    BillId = b.BillId,
                    PatientName = b.PatientName,
                    Amount = b.Amount,
                    IsPaid = b.IsPaid,
                    BillDate = b.BillDate.ToString("yyyy-MM-dd")
                })
            };
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bill history");
            return Page();
        }
    }
}

using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = ClinicManagement.Domain.Entities;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>View doctors page model (migrated from ViewDoctors.aspx.cs).</summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<DomainEntities.Doctor> Doctors { get; set; } = new List<DomainEntities.Doctor>();

    public ViewDoctorsModel(IDoctorService doctorService, ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? search)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Index");

        SearchQuery = search ?? string.Empty;

        try
        {
            Doctors = await _doctorService.GetAllDoctorsAsync(SearchQuery);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors list");
        }

        return Page();
    }
}

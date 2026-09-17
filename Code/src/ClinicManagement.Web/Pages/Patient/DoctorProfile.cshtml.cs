using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = ClinicManagement.Domain.Entities;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Doctor profile page model (migrated from DoctorProfile.aspx.cs).</summary>
public class DoctorProfileModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DomainEntities.Doctor? Doctor { get; set; }

    public DoctorProfileModel(IDoctorService doctorService, ILogger<DoctorProfileModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(int id)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Index");

        try
        {
            Doctor = await _doctorService.GetDoctorByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile for doctor {DoctorId}", id);
        }

        return Page();
    }
}

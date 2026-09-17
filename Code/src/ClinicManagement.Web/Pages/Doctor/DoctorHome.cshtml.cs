using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>Doctor home page model (migrated from DoctorHome.aspx.cs).</summary>
public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public string DoctorName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;

    public DoctorHomeModel(IDoctorService doctorService, ILogger<DoctorHomeModel> logger)
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
            var doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
            if (doctor != null)
            {
                DoctorName = doctor.Name;
                Phone = doctor.Phone;
                DeptName = doctor.DeptName;
                Specialization = doctor.Specialization;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for doctor {DoctorId}", userId);
        }

        return Page();
    }
}

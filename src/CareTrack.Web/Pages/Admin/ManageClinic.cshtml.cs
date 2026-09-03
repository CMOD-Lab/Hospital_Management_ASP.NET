using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = CareTrack.Domain.Entities;

namespace CareTrack.Web.Pages.Admin;

/// <summary>
/// Page model for managing clinic resources (doctors, patients, staff).
/// </summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public IEnumerable<DomainEntities.Doctor> Doctors { get; set; } = new List<DomainEntities.Doctor>();
    public IEnumerable<DomainEntities.Patient> Patients { get; set; } = new List<DomainEntities.Patient>();
    public IEnumerable<DomainEntities.OtherStaff> Staff { get; set; } = new List<DomainEntities.OtherStaff>();
    public string DoctorSearch { get; set; } = string.Empty;
    public string PatientSearch { get; set; } = string.Empty;
    public string StaffSearch { get; set; } = string.Empty;
    public string ActiveTab { get; set; } = "doctors";
    public string? StatusMessage { get; set; }

    public ManageClinicModel(IAdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(
        string? doctorSearch, string? patientSearch, string? staffSearch,
        string? activeTab, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        DoctorSearch = doctorSearch ?? string.Empty;
        PatientSearch = patientSearch ?? string.Empty;
        StaffSearch = staffSearch ?? string.Empty;
        ActiveTab = activeTab ?? "doctors";

        try
        {
            Doctors = await _adminService.GetDoctorsAsync(DoctorSearch, cancellationToken);
            Patients = await _adminService.GetPatientsAsync(PatientSearch, cancellationToken);
            Staff = await _adminService.GetStaffAsync(StaffSearch, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic page");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _adminService.DeleteDoctorAsync(doctorId, cancellationToken);
        StatusMessage = success ? "Doctor removed successfully." : "Error removing doctor.";

        return RedirectToPage(new { activeTab = "doctors" });
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        var success = await _adminService.DeleteStaffAsync(staffId, cancellationToken);
        StatusMessage = success ? "Staff member removed successfully." : "Error removing staff member.";

        return RedirectToPage(new { activeTab = "staff" });
    }
}

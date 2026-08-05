using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Manage clinic page model - view/delete doctors, patients, staff.</summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicModel(IAdminService adminService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public string? Message { get; set; }
    public string DoctorSearch { get; set; } = string.Empty;
    public string PatientSearch { get; set; } = string.Empty;
    public string StaffSearch { get; set; } = string.Empty;
    public IEnumerable<DoctorListItemDto> Doctors { get; set; } = new List<DoctorListItemDto>();
    public IEnumerable<PatientListItemDto> Patients { get; set; } = new List<PatientListItemDto>();
    public IEnumerable<StaffListItemDto> Staff { get; set; } = new List<StaffListItemDto>();

    public async Task<IActionResult> OnGetAsync(
        string? doctorSearch, string? patientSearch, string? staffSearch,
        CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        DoctorSearch = doctorSearch ?? string.Empty;
        PatientSearch = patientSearch ?? string.Empty;
        StaffSearch = staffSearch ?? string.Empty;

        Doctors = await _adminService.GetDoctorsAsync(DoctorSearch, cancellationToken);
        Patients = await _adminService.GetPatientsAsync(PatientSearch, cancellationToken);
        Staff = await _adminService.GetStaffAsync(StaffSearch, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        bool success = await _adminService.DeleteDoctorAsync(doctorId, cancellationToken);
        Message = success ? "Doctor removed successfully." : "Failed to remove doctor.";

        Doctors = await _adminService.GetDoctorsAsync(string.Empty, cancellationToken);
        Patients = await _adminService.GetPatientsAsync(string.Empty, cancellationToken);
        Staff = await _adminService.GetStaffAsync(string.Empty, cancellationToken);

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        bool success = await _adminService.DeleteStaffAsync(staffId, cancellationToken);
        Message = success ? "Staff member removed successfully." : "Failed to remove staff member.";

        Doctors = await _adminService.GetDoctorsAsync(string.Empty, cancellationToken);
        Patients = await _adminService.GetPatientsAsync(string.Empty, cancellationToken);
        Staff = await _adminService.GetStaffAsync(string.Empty, cancellationToken);

        return Page();
    }
}

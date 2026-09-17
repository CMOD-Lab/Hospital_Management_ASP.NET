using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = ClinicManagement.Domain.Entities;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Manage clinic page model (migrated from ManageClinic.aspx.cs).</summary>
public class ManageClinicModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly IPatientService _patientService;
    private readonly ILogger<ManageClinicModel> _logger;

    public string Message { get; set; } = string.Empty;
    public string DoctorSearch { get; set; } = string.Empty;
    public string PatientSearch { get; set; } = string.Empty;
    public string StaffSearch { get; set; } = string.Empty;
    public IEnumerable<DomainEntities.Doctor> Doctors { get; set; } = new List<DomainEntities.Doctor>();
    public IEnumerable<DomainEntities.Patient> Patients { get; set; } = new List<DomainEntities.Patient>();
    public IEnumerable<DomainEntities.Staff> StaffList { get; set; } = new List<DomainEntities.Staff>();

    public ManageClinicModel(IAdminService adminService, IPatientService patientService, ILogger<ManageClinicModel> logger)
    {
        _adminService = adminService;
        _patientService = patientService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? doctorSearch, string? patientSearch, string? staffSearch)
    {
        if (HttpContext.Session.GetInt32("UserType") != 3)
            return RedirectToPage("/Index");

        DoctorSearch = doctorSearch ?? string.Empty;
        PatientSearch = patientSearch ?? string.Empty;
        StaffSearch = staffSearch ?? string.Empty;

        try
        {
            Doctors = await _adminService.GetAllDoctorsAsync(DoctorSearch);
            Patients = await _patientService.GetAllPatientsAsync(PatientSearch);
            StaffList = await _adminService.GetAllStaffAsync(StaffSearch);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic page");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId)
    {
        await _adminService.DeleteDoctorAsync(doctorId);
        Message = "Doctor removed successfully.";
        return await OnGetAsync(null, null, null);
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId)
    {
        await _adminService.DeleteStaffAsync(staffId);
        Message = "Staff member removed successfully.";
        return await OnGetAsync(null, null, null);
    }
}

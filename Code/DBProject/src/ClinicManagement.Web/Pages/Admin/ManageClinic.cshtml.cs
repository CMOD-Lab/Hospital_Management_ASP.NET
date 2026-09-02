using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for managing clinic (doctors and staff).
/// </summary>
public class ManageClinicModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IStaffService _staffService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicModel(
        IDoctorService doctorService,
        IStaffService staffService,
        ILogger<ManageClinicModel> logger)
    {
        _doctorService = doctorService;
        _staffService = staffService;
        _logger = logger;
    }

    public string SearchQuery { get; set; } = string.Empty;
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    public IEnumerable<StaffViewModel> Staff { get; set; } = new List<StaffViewModel>();
    public string? Message { get; set; }

    public async Task<IActionResult> OnGetAsync(string? searchQuery = null)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        SearchQuery = searchQuery ?? string.Empty;

        try
        {
            var doctors = string.IsNullOrEmpty(SearchQuery)
                ? await _doctorService.GetAllDoctorsAsync()
                : await _doctorService.SearchDoctorsAsync(SearchQuery);

            var staff = string.IsNullOrEmpty(SearchQuery)
                ? await _staffService.GetAllStaffAsync()
                : await _staffService.SearchStaffAsync(SearchQuery);

            Doctors = doctors.Select(d => new DoctorViewModel
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName ?? string.Empty,
                Status = d.Status
            });

            Staff = staff.Select(s => new StaffViewModel
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading manage clinic page");
            Message = "Error loading data.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostDeleteDoctorAsync(int doctorId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        var success = await _doctorService.DeleteDoctorAsync(doctorId);
        Message = success ? "Doctor removed successfully." : "Error removing doctor.";
        return await OnGetAsync();
    }

    public async Task<IActionResult> OnPostDeleteStaffAsync(int staffId)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        var success = await _staffService.DeleteStaffAsync(staffId);
        Message = success ? "Staff member removed successfully." : "Error removing staff member.";
        return await OnGetAsync();
    }
}

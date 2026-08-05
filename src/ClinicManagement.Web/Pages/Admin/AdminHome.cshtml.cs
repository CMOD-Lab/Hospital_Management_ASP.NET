using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Admin home dashboard page model.</summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentSummaryDto> Departments { get; set; } = new List<DepartmentSummaryDto>();
    public IEnumerable<AppointmentSummaryDto> Appointments { get; set; } = new List<AppointmentSummaryDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        // Verify admin session
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            var homeInfo = await _adminService.GetAdminHomeInformationAsync(cancellationToken);
            TotalDoctors = homeInfo.TotalDoctors;
            TotalPatients = homeInfo.TotalPatients;
            TotalIncome = homeInfo.TotalIncome;
            Departments = homeInfo.Departments;
            Appointments = homeInfo.Appointments;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin home page");
        }

        return Page();
    }
}

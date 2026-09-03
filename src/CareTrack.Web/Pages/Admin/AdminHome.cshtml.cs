using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Admin;

/// <summary>
/// Page model for the admin dashboard.
/// </summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentStat> DepartmentStats { get; set; } = new List<DepartmentStat>();
    public IEnumerable<AppointmentStat> AppointmentStats { get; set; } = new List<AppointmentStat>();

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        try
        {
            var dashboardData = await _adminService.GetDashboardDataAsync(cancellationToken);

            TotalDoctors = dashboardData.TotalDoctors;
            TotalPatients = dashboardData.TotalPatients;
            TotalIncome = dashboardData.TotalIncome;

            // Manually map from domain DTOs to page ViewModels
            DepartmentStats = dashboardData.DepartmentStats.Select(d => new DepartmentStat
            {
                DeptName = d.DeptName,
                DoctorCount = d.DoctorCount
            }).ToList();

            AppointmentStats = dashboardData.AppointmentStats.Select(a => new AppointmentStat
            {
                AppointId = a.AppointId,
                PatientName = a.PatientName,
                DoctorName = a.DoctorName,
                Date = a.Date,
                Status = a.Status
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }

        return Page();
    }
}

// ViewModel classes for the page
public class DepartmentStat
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}

public class AppointmentStat
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

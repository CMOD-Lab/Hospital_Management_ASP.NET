using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Admin home dashboard page model (migrated from AdminHome.aspx.cs).</summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public decimal TotalIncome { get; set; }
    public IEnumerable<Department> Departments { get; set; } = new List<Department>();
    public IEnumerable<Appointment> Appointments { get; set; } = new List<Appointment>();

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        int? userId = HttpContext.Session.GetInt32("UserId");
        int? userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
            return RedirectToPage("/Index");

        try
        {
            var (totalDoctors, totalPatients, totalIncome) = await _adminService.GetDashboardStatsAsync();
            TotalDoctors = totalDoctors;
            TotalPatients = totalPatients;
            TotalIncome = totalIncome;

            Departments = await _adminService.GetDepartmentsAsync();
            Appointments = await _adminService.GetAllAppointmentsAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
        }

        return Page();
    }
}

using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Admin home page model</summary>
public class AdminHomeModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AdminHomeModel> _logger;

    public AdminHomeViewModel Dashboard { get; set; } = new();

    public AdminHomeModel(IAdminService adminService, ILogger<AdminHomeModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        try
        {
            var data = await _adminService.GetDashboardDataAsync(cancellationToken);

            // Manual ViewModel mapping from DTO
            Dashboard = new AdminHomeViewModel
            {
                TotalDoctors = data.TotalDoctors,
                TotalPatients = data.TotalPatients,
                TotalIncome = data.TotalIncome,
                DepartmentStats = data.DepartmentStats.Select(d => new DepartmentStatsItemViewModel
                {
                    DeptName = d.DeptName,
                    DoctorCount = d.DoctorCount,
                    PatientCount = d.PatientCount
                }),
                RecentAppointments = data.RecentAppointments.Select(a => new AppointmentItemViewModel
                {
                    AppointmentId = a.AppointmentId,
                    PatientName = a.PatientName,
                    Timings = a.Timings,
                    Status = a.Status,
                    AppointmentDate = a.AppointmentDate.ToString("yyyy-MM-dd")
                })
            };

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading admin dashboard");
            return Page();
        }
    }
}

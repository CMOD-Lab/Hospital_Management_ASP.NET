using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>
/// Page model for viewing doctors.
/// </summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(
        IDoctorService doctorService,
        IDepartmentService departmentService,
        ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public IEnumerable<DoctorViewModel> Doctors { get; set; } = new List<DoctorViewModel>();
    public string SelectedDepartment { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string? deptName = null)
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/Account/Login");

        SelectedDepartment = deptName ?? string.Empty;

        try
        {
            var departments = await _departmentService.GetAllDepartmentsAsync();
            Departments = departments.Select(d => new DepartmentViewModel
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName
            });

            var doctors = string.IsNullOrEmpty(SelectedDepartment)
                ? await _doctorService.GetAllDoctorsAsync()
                : await _doctorService.GetDoctorsByDepartmentAsync(SelectedDepartment);

            Doctors = doctors.Select(d => new DoctorViewModel
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.Department?.DeptName ?? string.Empty,
                Specialization = d.Specialization,
                ChargesPerVisit = d.ChargesPerVisit
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
        }

        return Page();
    }
}

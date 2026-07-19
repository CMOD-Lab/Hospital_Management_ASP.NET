using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>View doctors page model</summary>
public class ViewDoctorsModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsViewModel DoctorsModel { get; set; } = new();

    public ViewDoctorsModel(
        IDoctorService doctorService,
        IDepartmentService departmentService,
        ILogger<ViewDoctorsModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string deptName = "", CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentService.GetAllAsync(cancellationToken);
            DoctorsModel.Departments = departments.Select(d => new DepartmentItemViewModel
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName
            });

            DoctorsModel.SelectedDepartment = deptName;

            if (!string.IsNullOrEmpty(deptName))
            {
                var doctors = await _doctorService.GetByDepartmentAsync(deptName, cancellationToken);
                DoctorsModel.Doctors = doctors.Select(d => new DoctorItemViewModel
                {
                    DoctorId = d.DoctorId,
                    Name = d.Name,
                    DepartmentName = d.DepartmentName,
                    ChargesPerVisit = d.ChargesPerVisit,
                    ReputeIndex = d.ReputeIndex,
                    Specialization = d.Specialization
                });
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading view doctors page");
            return Page();
        }
    }
}

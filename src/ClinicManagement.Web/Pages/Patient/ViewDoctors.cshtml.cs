using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Patient;

public class ViewDoctorsModel : PageModel
{
    private readonly IPatientService _patientService;
    private readonly ILogger<ViewDoctorsModel> _logger;

    public ViewDoctorsModel(IPatientService patientService, ILogger<ViewDoctorsModel> logger)
    {
        _patientService = patientService;
        _logger = logger;
    }

    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    public IEnumerable<DoctorListItemDto> Doctors { get; set; } = new List<DoctorListItemDto>();
    public string SelectedDept { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string? deptName, CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 1) return RedirectToPage("/SignUp");

        Departments = await _patientService.GetDepartmentInfoAsync(cancellationToken);
        SelectedDept = deptName ?? string.Empty;

        if (!string.IsNullOrEmpty(deptName))
        {
            Doctors = await _patientService.GetDoctorsByDepartmentAsync(deptName, cancellationToken);
        }

        return Page();
    }
}

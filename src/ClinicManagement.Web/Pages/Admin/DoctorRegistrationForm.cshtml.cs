using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Doctor registration form page model.</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public DoctorRegistrationFormModel(IAdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public IEnumerable<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();

    public async Task<IActionResult> OnGetAsync(CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        Departments = await _adminService.GetDepartmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string Email, string Password, string BirthDate,
        int DeptNo, string Phone, string Gender, string Address,
        int Experience, int Salary, int ChargesPerVisit,
        string Specialization, string Qualification,
        CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        Departments = await _adminService.GetDepartmentsAsync(cancellationToken);

        // Validate department selection
        if (DeptNo == 0)
        {
            ErrorMessage = "Please select a department.";
            return Page();
        }

        // Check if email already exists
        bool emailExists = await _adminService.CheckDoctorEmailExistsAsync(Email, cancellationToken);
        if (emailExists)
        {
            ErrorMessage = "This email already exists. Please choose a different one.";
            return Page();
        }

        var dto = new AddDoctorDto
        {
            Name = Name,
            Email = Email,
            Password = Password,
            BirthDate = BirthDate,
            DeptNo = DeptNo,
            Phone = Phone ?? string.Empty,
            Gender = string.IsNullOrEmpty(Gender) ? 'M' : Gender[0],
            Address = Address ?? string.Empty,
            Experience = Experience,
            Salary = Salary,
            ChargesPerVisit = ChargesPerVisit,
            Specialization = Specialization ?? string.Empty,
            Qualification = Qualification
        };

        bool success = await _adminService.AddDoctorAsync(dto, cancellationToken);

        if (success)
        {
            SuccessMessage = "Doctor added successfully!";
            _logger.LogInformation("Doctor registered: {Name}", Name);
        }
        else
        {
            ErrorMessage = "Failed to add doctor. Please try again.";
        }

        return Page();
    }
}

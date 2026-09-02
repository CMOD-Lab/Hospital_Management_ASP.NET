using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DoctorEntity = ClinicManagement.Domain.Entities.Doctor;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for doctor registration form.
/// </summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public DoctorRegistrationFormModel(
        IDoctorService doctorService,
        IDepartmentService departmentService,
        ILogger<DoctorRegistrationFormModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    [BindProperty]
    public DoctorRegistrationViewModel Input { get; set; } = new();

    public IEnumerable<DepartmentViewModel> Departments { get; set; } = new List<DepartmentViewModel>();
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        await LoadDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        await LoadDepartmentsAsync();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Check if email already exists
            if (await _doctorService.EmailExistsAsync(Input.Email))
            {
                ErrorMessage = "A doctor with this email already exists.";
                return Page();
            }

            var doctor = new DoctorEntity
            {
                Name = Input.Name,
                BirthDate = DateTime.Parse(Input.BirthDate),
                DeptNo = Input.DeptNo,
                Phone = Input.Phone,
                Gender = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0],
                Address = Input.Address,
                WorkExperience = Input.WorkExperience,
                Salary = Input.Salary,
                ChargesPerVisit = Input.ChargesPerVisit,
                Specialization = Input.Specialization,
                Qualification = Input.Qualification,
                Status = 1
            };

            var success = await _doctorService.AddDoctorAsync(doctor, Input.Email, Input.Password);

            if (success)
            {
                SuccessMessage = "Doctor registered successfully!";
                Input = new DoctorRegistrationViewModel();
            }
            else
            {
                ErrorMessage = "Failed to register doctor. Please try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            ErrorMessage = "An error occurred during registration.";
        }

        return Page();
    }

    private async Task LoadDepartmentsAsync()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();
        Departments = departments.Select(d => new DepartmentViewModel
        {
            DeptNo = d.DeptNo,
            DeptName = d.DeptName
        });
        Input.Departments = Departments;
    }
}

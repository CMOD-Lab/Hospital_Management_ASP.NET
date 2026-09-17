using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = ClinicManagement.Domain.Entities;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Doctor registration form page model (migrated from DoctorRegistrationForm.aspx.cs).</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;
    public IEnumerable<DomainEntities.Department> Departments { get; set; } = new List<DomainEntities.Department>();

    public DoctorRegistrationFormModel(IAdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (HttpContext.Session.GetInt32("UserType") != 3)
            return RedirectToPage("/Index");

        Departments = await _adminService.GetDepartmentsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string Email, string Password, string BirthDate,
        int DeptNo, string Phone, string Gender, string Address,
        int Experience, int Salary, int ChargesPerVisit,
        string Specialization, string Qualification)
    {
        Departments = await _adminService.GetDepartmentsAsync();

        if (await _adminService.CheckDoctorEmailExistsAsync(Email))
        {
            ErrorMessage = "A doctor with this email already exists.";
            return Page();
        }

        var doctor = new DomainEntities.Doctor
        {
            Name = Name,
            Email = Email,
            Password = Password,
            BirthDate = BirthDate,
            DeptNo = DeptNo,
            Phone = Phone,
            Gender = Gender,
            Address = Address,
            Experience = Experience,
            Salary = Salary,
            ChargesPerVisit = ChargesPerVisit,
            Specialization = Specialization,
            Qualification = Qualification
        };

        bool success = await _adminService.AddDoctorAsync(doctor);
        if (success)
        {
            SuccessMessage = $"Doctor '{Name}' has been registered successfully.";
        }
        else
        {
            ErrorMessage = "Failed to register doctor. Please try again.";
        }

        return Page();
    }
}

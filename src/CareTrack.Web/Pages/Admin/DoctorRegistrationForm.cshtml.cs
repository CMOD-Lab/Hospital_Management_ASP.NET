using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = CareTrack.Domain.Entities;

namespace CareTrack.Web.Pages.Admin;

/// <summary>
/// Page model for doctor registration form.
/// </summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    public IEnumerable<DomainEntities.Department> Departments { get; set; } = new List<DomainEntities.Department>();
    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public DoctorRegistrationFormModel(IAdminService adminService, ILogger<DoctorRegistrationFormModel> logger)
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

        Departments = await _adminService.GetDepartmentsAsync(cancellationToken);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string email, string password, string birthDate,
        string phone, string address, string gender, int deptNo,
        string qualification, string specialization, int workExperience,
        double monthlySalary, double chargesPerVisit, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        Departments = await _adminService.GetDepartmentsAsync(cancellationToken);

        if (!DateTime.TryParse(birthDate, out var parsedDate))
        {
            StatusMessage = "Invalid birth date format.";
            IsSuccess = false;
            return Page();
        }

        var doctor = new DomainEntities.Doctor
        {
            Name = name,
            BirthDate = parsedDate,
            Phone = phone,
            Address = address,
            Gender = gender,
            DeptNo = deptNo,
            Qualification = qualification,
            Specialization = specialization,
            WorkExperience = workExperience,
            MonthlySalary = monthlySalary,
            ChargesPerVisit = chargesPerVisit,
            Status = 1
        };

        bool addSuccess;
        string addMessage;
        (addSuccess, addMessage) = await _adminService.AddDoctorAsync(doctor, email, password, cancellationToken);

        StatusMessage = addMessage;
        IsSuccess = addSuccess;

        if (addSuccess)
        {
            _logger.LogInformation("Doctor registered successfully: {Name}", name);
        }

        return Page();
    }
}

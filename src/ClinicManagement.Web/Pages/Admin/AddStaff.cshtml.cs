using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Add staff member page model.</summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string BirthDate, string Phone, string Gender,
        string Address, int Salary, string Qualification, string Designation,
        CancellationToken cancellationToken)
    {
        int? userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/SignUp");

        var dto = new AddStaffDto
        {
            Name = Name,
            BirthDate = BirthDate ?? string.Empty,
            Phone = Phone ?? string.Empty,
            Gender = string.IsNullOrEmpty(Gender) ? 'M' : Gender[0],
            Address = Address ?? string.Empty,
            Salary = Salary,
            Qualification = Qualification ?? string.Empty,
            Designation = Designation
        };

        bool success = await _adminService.AddStaffAsync(dto, cancellationToken);

        if (success)
        {
            SuccessMessage = "Staff member added successfully!";
            _logger.LogInformation("Staff member added: {Name}", Name);
        }
        else
        {
            ErrorMessage = "Failed to add staff member. Please try again.";
        }

        return Page();
    }
}

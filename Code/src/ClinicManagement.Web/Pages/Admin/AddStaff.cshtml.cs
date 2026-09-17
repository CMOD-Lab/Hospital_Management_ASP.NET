using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Add staff page model (migrated from AddStaff.aspx.cs).</summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public string ErrorMessage { get; set; } = string.Empty;
    public string SuccessMessage { get; set; } = string.Empty;

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        if (HttpContext.Session.GetInt32("UserType") != 3)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string Name, string BirthDate, string Phone, string Gender,
        string Address, int Salary, string Qualification, string Designation)
    {
        var staff = new Staff
        {
            Name = Name,
            BirthDate = BirthDate,
            Phone = Phone,
            Gender = Gender,
            Address = Address,
            Salary = Salary,
            Qualification = Qualification,
            Designation = Designation
        };

        bool success = await _adminService.AddStaffAsync(staff);
        if (success)
        {
            SuccessMessage = $"Staff member '{Name}' has been added successfully.";
        }
        else
        {
            ErrorMessage = "Failed to add staff member. Please try again.";
        }

        return Page();
    }
}

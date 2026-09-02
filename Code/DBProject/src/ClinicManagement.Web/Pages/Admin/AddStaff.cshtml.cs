using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>
/// Page model for adding staff members.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly IStaffService _staffService;
    private readonly ILogger<AddStaffModel> _logger;

    public AddStaffModel(IStaffService staffService, ILogger<AddStaffModel> logger)
    {
        _staffService = staffService;
        _logger = logger;
    }

    [BindProperty]
    public AddStaffViewModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public IActionResult OnGet()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        if (userType != 3) return RedirectToPage("/Account/Login");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            var staff = new OtherStaff
            {
                Name = Input.Name,
                BirthDate = DateTime.Parse(Input.BirthDate),
                Phone = Input.Phone,
                Gender = string.IsNullOrEmpty(Input.Gender) ? 'M' : Input.Gender[0],
                Address = Input.Address,
                Salary = Input.Salary,
                Qualification = Input.Qualification,
                Designation = Input.Designation
            };

            var success = await _staffService.AddStaffAsync(staff);

            if (success)
            {
                SuccessMessage = "Staff member added successfully!";
                Input = new AddStaffViewModel();
            }
            else
            {
                ErrorMessage = "Failed to add staff member.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            ErrorMessage = "An error occurred while adding staff member.";
        }

        return Page();
    }
}

using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareTrack.Web.Pages.Admin;

/// <summary>
/// Page model for adding staff members.
/// </summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    public string? StatusMessage { get; set; }
    public bool IsSuccess { get; set; }

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public IActionResult OnGet()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(
        string name, string? birthDate, string? phone, string? address,
        string gender, string designation, string? qualification,
        double salary, CancellationToken cancellationToken)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var userType = HttpContext.Session.GetInt32("UserType");

        if (userId == null || userType != 3)
        {
            return RedirectToPage("/SignUp");
        }

        DateTime? parsedDate = null;
        if (!string.IsNullOrWhiteSpace(birthDate) && DateTime.TryParse(birthDate, out var d))
        {
            parsedDate = d;
        }

        var staff = new OtherStaff
        {
            Name = name,
            BirthDate = parsedDate,
            Phone = phone,
            Address = address,
            Gender = gender,
            Designation = designation,
            HighestQualification = qualification,
            Salary = salary
        };

        var (success, message) = await _adminService.AddStaffAsync(staff, cancellationToken);

        StatusMessage = message;
        IsSuccess = success;

        if (success)
        {
            _logger.LogInformation("Staff member added successfully: {Name}", name);
        }

        return Page();
    }
}

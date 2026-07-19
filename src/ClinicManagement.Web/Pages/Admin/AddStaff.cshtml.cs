using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Add staff page model</summary>
public class AddStaffModel : PageModel
{
    private readonly IAdminService _adminService;
    private readonly ILogger<AddStaffModel> _logger;

    [BindProperty]
    public AddStaffViewModel FormModel { get; set; } = new();

    public AddStaffModel(IAdminService adminService, ILogger<AddStaffModel> logger)
    {
        _adminService = adminService;
        _logger = logger;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Manual mapping from ViewModel to DTO
            var dto = new StaffCreateDto
            {
                Name = FormModel.Name,
                BirthDate = FormModel.BirthDate,
                Phone = FormModel.Phone,
                Gender = FormModel.Gender,
                Address = FormModel.Address,
                Salary = FormModel.Salary,
                Qualification = FormModel.Qualification,
                Designation = FormModel.Designation
            };

            var success = await _adminService.AddStaffAsync(dto, cancellationToken);

            if (success)
            {
                FormModel = new AddStaffViewModel { SuccessMessage = "Staff member added successfully!" };
            }
            else
            {
                FormModel.ErrorMessage = "There was an error adding the staff member. Please try again.";
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member");
            FormModel.ErrorMessage = "There was an error. Please try again.";
            return Page();
        }
    }
}

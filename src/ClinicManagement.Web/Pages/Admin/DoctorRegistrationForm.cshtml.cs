using ClinicManagement.Application.DTOs;
using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Doctor registration form page model</summary>
public class DoctorRegistrationFormModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IDepartmentService _departmentService;
    private readonly ILogger<DoctorRegistrationFormModel> _logger;

    [BindProperty]
    public DoctorRegistrationViewModel FormModel { get; set; } = new();

    public DoctorRegistrationFormModel(
        IDoctorService doctorService,
        IDepartmentService departmentService,
        ILogger<DoctorRegistrationFormModel> logger)
    {
        _doctorService = doctorService;
        _departmentService = departmentService;
        _logger = logger;
    }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await LoadDepartmentsAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        await LoadDepartmentsAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Check if email already exists
            if (await _doctorService.EmailExistsAsync(FormModel.Email, cancellationToken))
            {
                FormModel.ErrorMessage = "This email already exists. Kindly choose a different one!";
                return Page();
            }

            // Manual mapping from ViewModel to DTO
            var dto = new DoctorCreateDto
            {
                Name = FormModel.Name,
                Email = FormModel.Email,
                Password = FormModel.Password,
                BirthDate = FormModel.BirthDate,
                DeptNo = FormModel.DeptNo,
                Phone = FormModel.Phone,
                Gender = FormModel.Gender,
                Address = FormModel.Address,
                Experience = FormModel.Experience,
                Salary = FormModel.Salary,
                ChargesPerVisit = FormModel.ChargesPerVisit,
                Specialization = FormModel.Specialization,
                Qualification = FormModel.Qualification
            };

            var success = await _doctorService.AddDoctorAsync(dto, cancellationToken);

            if (success)
            {
                FormModel.SuccessMessage = "Doctor added successfully!";
                FormModel = new DoctorRegistrationViewModel { SuccessMessage = "Doctor added successfully!" };
                await LoadDepartmentsAsync(cancellationToken);
            }
            else
            {
                FormModel.ErrorMessage = "There was an error adding the doctor. Please try again.";
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering doctor");
            FormModel.ErrorMessage = "There was an error. Please try again.";
            return Page();
        }
    }

    private async Task LoadDepartmentsAsync(CancellationToken cancellationToken)
    {
        var departments = await _departmentService.GetAllAsync(cancellationToken);
        FormModel.Departments = departments.Select(d => new DepartmentItemViewModel
        {
            DeptNo = d.DeptNo,
            DeptName = d.DeptName
        });
    }
}

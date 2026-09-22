using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public class CreateModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public CreateModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [BindProperty]
    public DoctorInputModel Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new DoctorCreateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate, Input.Qualification, Input.Specialization, Input.YearsOfExperience, Input.Salary, Input.ChargesPerVisit, Input.DepartmentId);
        await _doctorService.CreateAsync(dto, cancellationToken);
        return RedirectToPage("Index");
    }

    public class DoctorInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        [DataType(DataType.Date)] public DateTime BirthDate { get; set; } = DateTime.UtcNow.Date;
        [Required] public string Qualification { get; set; } = string.Empty;
        [Required] public string Specialization { get; set; } = string.Empty;
        [Range(0, 50)] public int YearsOfExperience { get; set; }
        [Range(0, 100000)] public decimal Salary { get; set; }
        [Range(0, 10000)] public decimal ChargesPerVisit { get; set; }
        [Range(1, int.MaxValue)] public int DepartmentId { get; set; } = 1;
    }
}

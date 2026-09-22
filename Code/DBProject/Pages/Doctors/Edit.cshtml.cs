using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public class EditModel : PageModel
{
    private readonly IDoctorService _doctorService;

    public EditModel(IDoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [BindProperty]
    public DoctorInputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var doctor = await _doctorService.GetByIdAsync(id, cancellationToken);
        if (doctor is null)
        {
            return NotFound();
        }

        Input = new DoctorInputModel
        {
            Id = doctor.Id,
            Name = doctor.Name,
            Email = doctor.Email,
            PhoneNumber = doctor.PhoneNumber,
            Qualification = doctor.Qualification,
            Specialization = doctor.Specialization,
            YearsOfExperience = doctor.YearsOfExperience,
            ChargesPerVisit = doctor.ChargesPerVisit,
            DepartmentId = doctor.DepartmentId,
            Gender = "Unknown",
            Address = string.Empty,
            BirthDate = DateTime.UtcNow.Date,
            Salary = 0,
            IsActive = true
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dto = new DoctorUpdateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate, Input.Qualification, Input.Specialization, Input.YearsOfExperience, Input.Salary, Input.ChargesPerVisit, Input.DepartmentId, Input.IsActive);
        await _doctorService.UpdateAsync(Input.Id, dto, cancellationToken);
        return RedirectToPage("Index");
    }

    public class DoctorInputModel
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required, EmailAddress] public string Email { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        [Required] public string Qualification { get; set; } = string.Empty;
        [Required] public string Specialization { get; set; } = string.Empty;
        public int YearsOfExperience { get; set; }
        public decimal Salary { get; set; }
        public decimal ChargesPerVisit { get; set; }
        public int DepartmentId { get; set; }
        public bool IsActive { get; set; }
    }
}

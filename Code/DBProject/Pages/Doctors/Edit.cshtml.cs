using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.Doctors;

public sealed class EditModel : CreateModel
{
    private readonly IDoctorService _doctorService;
    public EditModel(IDoctorService service) : base(service) => _doctorService = service;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var item = await _doctorService.GetByIdAsync(id, cancellationToken);
        if (item is null) return NotFound();
        Input = new DoctorInputModel
        {
            Name = item.Name,
            Email = item.Email,
            PhoneNumber = item.PhoneNumber,
            Gender = item.Gender,
            Qualification = item.Qualification,
            Specialization = item.Specialization,
            Address = item.Address,
            ChargesPerVisit = item.ChargesPerVisit,
            ExperienceYears = item.ExperienceYears,
            DepartmentId = item.DepartmentId
        };
        return Page();
    }

    public new async Task<IActionResult> OnPostAsync(int id, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        var dto = new DoctorUpdateDto(Input.Name, Input.Email, Input.PhoneNumber, Input.Gender, Input.Qualification, Input.Specialization, Input.Address, Input.ChargesPerVisit, Input.ExperienceYears, Input.DepartmentId);
        await _doctorService.UpdateAsync(id, dto, cancellationToken);
        return RedirectToPage("Index");
    }
}

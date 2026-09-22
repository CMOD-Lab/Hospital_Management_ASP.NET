using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.StaffMembers;

public class EditModel : PageModel
{
    private readonly IStaffMemberService _service;
    public EditModel(IStaffMemberService service) => _service = service;
    [BindProperty] public StaffInputModel Input { get; set; } = new();
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        var staff = await _service.GetByIdAsync(id, cancellationToken);
        if (staff is null) return NotFound();
        Input = new StaffInputModel { Id = staff.Id, Name = staff.Name, PhoneNumber = staff.PhoneNumber, Gender = staff.Gender, Address = staff.Address, BirthDate = staff.BirthDate, Qualification = staff.Qualification, Designation = staff.Designation, Salary = staff.Salary, IsActive = true };
        return Page();
    }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _service.UpdateAsync(Input.Id, new StaffMemberUpdateDto(Input.Name, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate, Input.Qualification, Input.Designation, Input.Salary, Input.IsActive), cancellationToken);
        return RedirectToPage("Index");
    }
    public class StaffInputModel
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        [Required] public string Qualification { get; set; } = string.Empty;
        [Required] public string Designation { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public bool IsActive { get; set; }
    }
}

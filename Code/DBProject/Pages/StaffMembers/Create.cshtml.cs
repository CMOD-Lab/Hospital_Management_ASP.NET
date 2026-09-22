using System.ComponentModel.DataAnnotations;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.StaffMembers;

public class CreateModel : PageModel
{
    private readonly IStaffMemberService _service;
    public CreateModel(IStaffMemberService service) => _service = service;
    [BindProperty] public StaffInputModel Input { get; set; } = new();
    public void OnGet() { }
    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        await _service.CreateAsync(new StaffMemberCreateDto(Input.Name, Input.PhoneNumber, Input.Gender, Input.Address, Input.BirthDate, Input.Qualification, Input.Designation, Input.Salary), cancellationToken);
        return RedirectToPage("Index");
    }
    public class StaffInputModel
    {
        [Required] public string Name { get; set; } = string.Empty;
        [Required] public string PhoneNumber { get; set; } = string.Empty;
        [Required] public string Gender { get; set; } = string.Empty;
        [Required] public string Address { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; } = DateTime.UtcNow.Date;
        [Required] public string Qualification { get; set; } = string.Empty;
        [Required] public string Designation { get; set; } = string.Empty;
        public decimal Salary { get; set; }
    }
}

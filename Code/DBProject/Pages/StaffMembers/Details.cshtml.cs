using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.StaffMembers;

public class DetailsModel : PageModel
{
    private readonly IStaffMemberService _service;
    public DetailsModel(IStaffMemberService service) => _service = service;
    public StaffMemberDto? StaffMember { get; private set; }
    public async Task<IActionResult> OnGetAsync(int id, CancellationToken cancellationToken)
    {
        StaffMember = await _service.GetByIdAsync(id, cancellationToken);
        return StaffMember is null ? NotFound() : Page();
    }
}

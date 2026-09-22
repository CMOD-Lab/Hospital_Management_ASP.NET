using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages.StaffMembers;

public class IndexModel : PageModel
{
    private readonly IStaffMemberService _service;
    public IndexModel(IStaffMemberService service) => _service = service;
    public IReadOnlyList<StaffMemberDto> StaffMembers { get; private set; } = Array.Empty<StaffMemberDto>();
    [BindProperty(SupportsGet = true)] public string Search { get; set; } = string.Empty;
    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        StaffMembers = string.IsNullOrWhiteSpace(Search) ? await _service.GetAllAsync(cancellationToken) : await _service.SearchAsync(Search, cancellationToken);
    }
}

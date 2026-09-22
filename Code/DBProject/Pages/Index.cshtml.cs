using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagementSystem.Web.Pages;

public class IndexModel : PageModel
{
    private readonly IAppointmentService _appointmentService;

    public IndexModel(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    public DashboardDto Dashboard { get; private set; } = new();

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        Dashboard = await _appointmentService.GetDashboardAsync(cancellationToken);
    }
}

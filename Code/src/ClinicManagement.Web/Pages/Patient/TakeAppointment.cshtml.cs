using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DomainEntities = ClinicManagement.Domain.Entities;

namespace ClinicManagement.Web.Pages.Patient;

/// <summary>Take appointment page model (migrated from TakeAppointment.aspx.cs).</summary>
public class TakeAppointmentModel : PageModel
{
    private readonly IAppointmentService _appointmentService;
    private readonly IDoctorService _doctorService;
    private readonly ILogger<TakeAppointmentModel> _logger;

    public string Message { get; set; } = string.Empty;
    public string SelectedDept { get; set; } = string.Empty;
    public IEnumerable<DomainEntities.Department> Departments { get; set; } = new List<DomainEntities.Department>();
    public IEnumerable<DomainEntities.Doctor> DeptDoctors { get; set; } = new List<DomainEntities.Doctor>();

    public TakeAppointmentModel(IAppointmentService appointmentService, IDoctorService doctorService, ILogger<TakeAppointmentModel> logger)
    {
        _appointmentService = appointmentService;
        _doctorService = doctorService;
        _logger = logger;
    }

    public async Task<IActionResult> OnGetAsync(string? deptName)
    {
        if (HttpContext.Session.GetInt32("UserId") == null)
            return RedirectToPage("/Index");

        try
        {
            Departments = await _appointmentService.GetDepartmentsAsync();

            if (!string.IsNullOrEmpty(deptName))
            {
                SelectedDept = deptName;
                DeptDoctors = await _doctorService.GetDoctorsByDepartmentAsync(deptName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading take appointment page");
        }

        return Page();
    }
}

using ClinicManagement.Domain.Interfaces.Services;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Doctor;

/// <summary>
/// Page model for the doctor home page.
/// </summary>
public class DoctorHomeModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly ILogger<DoctorHomeModel> _logger;

    public DoctorHomeModel(IDoctorService doctorService, ILogger<DoctorHomeModel> logger)
    {
        _doctorService = doctorService;
        _logger = logger;
    }

    public DoctorHomeViewModel DoctorInfo { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var userType = HttpContext.Session.GetInt32("UserType");
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userType != 2 || !userId.HasValue)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(userId.Value);
            if (doctor == null)
            {
                return RedirectToPage("/Account/Login");
            }

            DoctorInfo = new DoctorHomeViewModel
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Address = doctor.Address,
                Gender = doctor.Gender.ToString(),
                DepartmentName = doctor.Department?.DeptName ?? string.Empty,
                ChargesPerVisit = doctor.ChargesPerVisit,
                WorkExperience = doctor.WorkExperience,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor home for doctor ID: {DoctorId}", userId);
        }

        return Page();
    }
}

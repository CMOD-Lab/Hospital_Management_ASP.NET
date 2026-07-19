using ClinicManagement.Application.Interfaces;
using ClinicManagement.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicManagement.Web.Pages.Admin;

/// <summary>Manage clinic page model</summary>
public class ManageClinicModel : PageModel
{
    private readonly IDoctorService _doctorService;
    private readonly IPatientService _patientService;
    private readonly IAdminService _adminService;
    private readonly ILogger<ManageClinicModel> _logger;

    public ManageClinicViewModel ClinicModel { get; set; } = new();

    public ManageClinicModel(
        IDoctorService doctorService,
        IPatientService patientService,
        IAdminService adminService,
        ILogger<ManageClinicModel> logger)
    {
        _doctorService = doctorService;
        _patientService = patientService;
        _adminService = adminService;
        _logger = logger;
    }

    public async Task OnGetAsync(
        string category = "DOCTOR",
        string searchQuery = "",
        string action = "",
        int id = 0,
        CancellationToken cancellationToken = default)
    {
        ClinicModel.Category = category;
        ClinicModel.SearchQuery = searchQuery;

        try
        {
            // Handle delete action
            if (action == "delete" && id > 0)
            {
                if (category == "DOCTOR")
                {
                    var deleted = await _doctorService.DeactivateDoctorAsync(id, cancellationToken);
                    ClinicModel.Message = deleted ? $"Doctor No: {id} Deleted" : "There was some error";
                }
                else if (category == "OTHERSTAFF")
                {
                    var deleted = await _adminService.DeleteStaffAsync(id, cancellationToken);
                    ClinicModel.Message = deleted ? $"Staff No: {id} Deleted" : "There was some error";
                }
                else
                {
                    ClinicModel.Message = "You are not authorized to delete a Patient";
                }
            }

            // Handle view action
            if (action == "view" && id > 0)
            {
                if (category == "DOCTOR")
                {
                    var doc = await _doctorService.GetByIdAsync(id, cancellationToken);
                    if (doc != null)
                    {
                        ClinicModel.SelectedDetails = $"<p><b>Name:</b> {doc.Name}</p>" +
                            $"<p><b>Phone:</b> {doc.Phone}</p>" +
                            $"<p><b>Gender:</b> {doc.Gender}</p>" +
                            $"<p><b>Qualification:</b> {doc.Qualification}</p>" +
                            $"<p><b>Age:</b> {doc.Age}</p>" +
                            $"<p><b>Charges:</b> {doc.ChargesPerVisit}</p>" +
                            $"<p><b>Repute Index:</b> {doc.ReputeIndex}</p>";
                    }
                }
                else if (category == "PATIENT")
                {
                    var patient = await _patientService.GetByIdAsync(id, cancellationToken);
                    if (patient != null)
                    {
                        ClinicModel.SelectedDetails = $"<p><b>Name:</b> {patient.Name}</p>" +
                            $"<p><b>Phone:</b> {patient.Phone}</p>" +
                            $"<p><b>Gender:</b> {patient.Gender}</p>" +
                            $"<p><b>Address:</b> {patient.Address}</p>" +
                            $"<p><b>Age:</b> {patient.Age}</p>";
                    }
                }
                else
                {
                    var staff = await _adminService.GetStaffByIdAsync(id, cancellationToken);
                    if (staff != null)
                    {
                        ClinicModel.SelectedDetails = $"<p><b>Name:</b> {staff.Name}</p>" +
                            $"<p><b>Phone:</b> {staff.Phone}</p>" +
                            $"<p><b>Gender:</b> {staff.Gender}</p>" +
                            $"<p><b>Address:</b> {staff.Address}</p>" +
                            $"<p><b>Salary:</b> {staff.Salary}</p>";
                    }
                }
            }

            // Load data based on category
            await LoadDataAsync(category, searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ManageClinic");
            ClinicModel.Message = "There was an error loading data.";
        }
    }

    private async Task LoadDataAsync(string category, string searchQuery, CancellationToken cancellationToken)
    {
        if (category == "DOCTOR")
        {
            var doctors = string.IsNullOrEmpty(searchQuery)
                ? await _doctorService.GetAllAsync(cancellationToken)
                : await _doctorService.SearchAsync(searchQuery, cancellationToken);

            ClinicModel.Doctors = doctors.Select(d => new DoctorItemViewModel
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                DepartmentName = d.DepartmentName
            });
        }
        else if (category == "PATIENT")
        {
            var patients = string.IsNullOrEmpty(searchQuery)
                ? await _patientService.GetAllAsync(cancellationToken)
                : await _patientService.SearchAsync(searchQuery, cancellationToken);

            ClinicModel.Patients = patients.Select(p => new PatientItemViewModel
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        else
        {
            var staff = string.IsNullOrEmpty(searchQuery)
                ? await _adminService.GetAllStaffAsync(cancellationToken)
                : await _adminService.SearchStaffAsync(searchQuery, cancellationToken);

            ClinicModel.Staff = staff.Select(s => new StaffItemViewModel
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
    }
}

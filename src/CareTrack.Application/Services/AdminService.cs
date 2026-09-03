using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CareTrack.Application.Services;

/// <summary>
/// Service implementation for admin operations.
/// </summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IAuthRepository _authRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        IAuthRepository authRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _authRepository = authRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets dashboard statistics for the admin home page.
    /// </summary>
    public async Task<AdminDashboardData> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin dashboard data");

            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);

            var doctorList = doctors.ToList();
            var patientList = patients.ToList();
            var deptList = departments.ToList();

            // Calculate total income from completed appointments
            double totalIncome = 0;
            foreach (var doc in doctorList)
            {
                var appointments = await _appointmentRepository.GetByDoctorIdAsync(doc.DoctorId, cancellationToken);
                totalIncome += appointments
                    .Where(a => a.BillStatus == "Paid" && a.BillAmount.HasValue)
                    .Sum(a => a.BillAmount!.Value);
            }

            var deptStats = deptList.Select(d => new DepartmentStat
            {
                DeptName = d.DeptName,
                DoctorCount = doctorList.Count(doc => doc.DeptNo == d.DeptNo)
            }).ToList();

            // Get recent appointments
            var recentAppointments = new List<AppointmentStat>();
            foreach (var doc in doctorList.Take(5))
            {
                var appts = await _appointmentRepository.GetByDoctorIdAsync(doc.DoctorId, cancellationToken);
                foreach (var appt in appts.Take(3))
                {
                    recentAppointments.Add(new AppointmentStat
                    {
                        AppointId = appt.AppointId,
                        DoctorName = doc.Name,
                        PatientName = appt.Patient?.Name ?? "Unknown",
                        Date = appt.Date,
                        Status = appt.AppointmentStatus switch
                        {
                            1 => "Approved",
                            2 => "Pending",
                            3 => "Completed",
                            4 => "Rejected",
                            _ => "Unknown"
                        }
                    });
                }
            }

            return new AdminDashboardData
            {
                TotalDoctors = doctorList.Count,
                TotalPatients = patientList.Count,
                TotalIncome = totalIncome,
                DepartmentStats = deptStats,
                AppointmentStats = recentAppointments.Take(10)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin dashboard data");
            return new AdminDashboardData();
        }
    }

    /// <summary>
    /// Gets doctors with optional search filter.
    /// </summary>
    public async Task<IEnumerable<Doctor>> GetDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return await _doctorRepository.GetAllActiveAsync(cancellationToken);

            return await _doctorRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors with query: {Query}", searchQuery);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <summary>
    /// Gets patients with optional search filter.
    /// </summary>
    public async Task<IEnumerable<Patient>> GetPatientsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return await _patientRepository.GetAllAsync(cancellationToken);

            return await _patientRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients with query: {Query}", searchQuery);
            return Enumerable.Empty<Patient>();
        }
    }

    /// <summary>
    /// Gets staff members with optional search filter.
    /// </summary>
    public async Task<IEnumerable<OtherStaff>> GetStaffAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchQuery))
                return await _staffRepository.GetAllAsync(cancellationToken);

            return await _staffRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with query: {Query}", searchQuery);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    /// <summary>
    /// Adds a new doctor to the system.
    /// </summary>
    public async Task<(bool Success, string Message)> AddDoctorAsync(Doctor doctor, string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor with email: {Email}", email);

            if (await _authRepository.EmailExistsAsync(email, cancellationToken))
            {
                return (false, "Email already exists.");
            }

            var login = new LoginTable
            {
                Email = email,
                Password = password,
                Type = 2 // Doctor
            };

            var createdLogin = await _authRepository.CreateLoginAsync(login, cancellationToken);
            doctor.DoctorId = createdLogin.LoginId;
            doctor.Status = 1; // Present

            await _doctorRepository.AddAsync(doctor, cancellationToken);

            _logger.LogInformation("Doctor added successfully with ID: {DoctorId}", doctor.DoctorId);
            return (true, "Doctor added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor with email: {Email}", email);
            return (false, "An error occurred while adding the doctor.");
        }
    }

    /// <summary>
    /// Adds a new staff member to the system.
    /// </summary>
    public async Task<(bool Success, string Message)> AddStaffAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {Name}", staff.Name);
            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff member added successfully");
            return (true, "Staff member added successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", staff.Name);
            return (false, "An error occurred while adding the staff member.");
        }
    }

    /// <summary>
    /// Soft-deletes a doctor (sets status to 0).
    /// </summary>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Soft-deleting doctor with ID: {DoctorId}", id);
            return await _doctorRepository.SoftDeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor with ID: {DoctorId}", id);
            return false;
        }
    }

    /// <summary>
    /// Deletes a staff member.
    /// </summary>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff member with ID: {StaffId}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member with ID: {StaffId}", id);
            return false;
        }
    }

    public async Task<Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID: {DoctorId}", id);
            return null;
        }
    }

    public async Task<OtherStaff?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _staffRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID: {StaffId}", id);
            return null;
        }
    }

    public async Task<Patient?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _patientRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {PatientId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _departmentRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Enumerable.Empty<Department>();
        }
    }
}

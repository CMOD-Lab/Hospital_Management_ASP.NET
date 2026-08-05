using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Handles all admin-related business operations.</summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IDepartmentRepository departmentRepository,
        IAppointmentRepository appointmentRepository,
        ILoginRepository loginRepository,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _departmentRepository = departmentRepository;
        _appointmentRepository = appointmentRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<AdminHomeDto> GetAdminHomeInformationAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving admin home information");

            var doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(0, cancellationToken);

            // Calculate total income from completed paid appointments
            var allAppointments = new List<Appointment>();
            foreach (var doc in doctors)
            {
                var docAppointments = await _appointmentRepository.GetByDoctorIdAsync(doc.DoctorId, cancellationToken);
                allAppointments.AddRange(docAppointments);
            }

            double totalIncome = allAppointments
                .Where(a => a.BillStatus == "Paid" && a.BillAmount.HasValue)
                .Sum(a => a.BillAmount!.Value);

            var deptSummaries = departments.Select(d => new DepartmentSummaryDto
            {
                DeptName = d.DeptName,
                DoctorCount = doctors.Count(doc => doc.DeptNo == d.DeptNo)
            });

            var appointmentSummaries = allAppointments
                .OrderByDescending(a => a.Date)
                .Take(20)
                .Select(a => new AppointmentSummaryDto
                {
                    AppointId = a.AppointId,
                    PatientName = a.Patient?.Name,
                    DoctorName = a.Doctor?.Name,
                    Date = a.Date,
                    Status = a.AppointmentStatus.HasValue ? a.AppointmentStatus.Value.ToString() : "Unknown"
                });

            return new AdminHomeDto
            {
                TotalDoctors = doctors.Count(),
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                Departments = deptSummaries,
                Appointments = appointmentSummaries
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin home information");
            return new AdminHomeDto();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddDoctorAsync(AddDoctorDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new doctor: {Name}", dto.Name);

            // Check email uniqueness
            bool emailExists = await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                _logger.LogWarning("Doctor email already exists: {Email}", dto.Email);
                return false;
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 2 // Doctor
            };
            int loginId = await _loginRepository.AddAsync(login, cancellationToken);

            // Parse birth date
            if (!DateTime.TryParse(dto.BirthDate, out DateTime birthDate))
            {
                _logger.LogWarning("Invalid birth date format: {BirthDate}", dto.BirthDate);
                return false;
            }

            var doctor = new Doctor
            {
                DoctorId = loginId,
                Name = dto.Name,
                BirthDate = birthDate,
                DeptNo = dto.DeptNo,
                Phone = dto.Phone,
                Gender = dto.Gender,
                Address = dto.Address,
                WorkExperience = dto.Experience,
                MonthlySalary = dto.Salary,
                ChargesPerVisit = dto.ChargesPerVisit,
                Specialization = dto.Specialization,
                Qualification = dto.Qualification,
                Status = 1,
                PatientsTreated = 0
            };

            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully: {DoctorId}", loginId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {Name}", dto.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> AddStaffAsync(AddStaffDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {Name}", dto.Name);

            if (!DateTime.TryParse(dto.BirthDate, out DateTime birthDate))
            {
                _logger.LogWarning("Invalid birth date format: {BirthDate}", dto.BirthDate);
                return false;
            }

            var staff = new OtherStaff
            {
                Name = dto.Name,
                BirthDate = birthDate,
                Phone = dto.Phone,
                Gender = dto.Gender,
                Address = dto.Address,
                Salary = dto.Salary,
                HighestQualification = dto.Qualification,
                Designation = dto.Designation
            };

            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff member added successfully: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", dto.Name);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Soft-deleting doctor: {DoctorId}", id);
            bool result = await _doctorRepository.SoftDeleteAsync(id, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting doctor: {DoctorId}", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff member: {StaffId}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member: {StaffId}", id);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorListItemDto>> GetDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Doctor> doctors;
            if (string.IsNullOrWhiteSpace(searchQuery))
                doctors = await _doctorRepository.GetAllActiveAsync(cancellationToken);
            else
                doctors = await _doctorRepository.SearchAsync(searchQuery, cancellationToken);

            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var deptDict = departments.ToDictionary(d => d.DeptNo, d => d.DeptName);

            return doctors.Select(d => new DoctorListItemDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = deptDict.TryGetValue(d.DeptNo, out var deptName) ? deptName : "Unknown"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors");
            return Enumerable.Empty<DoctorListItemDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PatientListItemDto>> GetPatientsAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<Patient> patients;
            if (string.IsNullOrWhiteSpace(searchQuery))
                patients = await _patientRepository.GetAllAsync(cancellationToken);
            else
                patients = await _patientRepository.SearchAsync(searchQuery, cancellationToken);

            return patients.Select(p => new PatientListItemDto
            {
                PatientId = p.PatientId,
                Name = p.Name,
                Phone = p.Phone
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patients");
            return Enumerable.Empty<PatientListItemDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<StaffListItemDto>> GetStaffAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            IEnumerable<OtherStaff> staff;
            if (string.IsNullOrWhiteSpace(searchQuery))
                staff = await _staffRepository.GetAllAsync(cancellationToken);
            else
                staff = await _staffRepository.SearchAsync(searchQuery, cancellationToken);

            return staff.Select(s => new StaffListItemDto
            {
                StaffId = s.StaffId,
                Name = s.Name,
                Designation = s.Designation
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff");
            return Enumerable.Empty<StaffListItemDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> CheckDoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _loginRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking doctor email: {Email}", email);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return departments.Select(d => new DepartmentDto
            {
                DeptNo = d.DeptNo,
                DeptName = d.DeptName,
                Description = d.Description
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}

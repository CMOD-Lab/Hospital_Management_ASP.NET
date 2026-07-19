using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Admin service implementation</summary>
public class AdminService : IAdminService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IBillRepository _billRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffRepository staffRepository,
        IBillRepository billRepository,
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        IMapper mapper,
        ILogger<AdminService> logger)
    {
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _billRepository = billRepository;
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Gets admin dashboard data</summary>
    public async Task<AdminDashboardDto> GetDashboardDataAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalDoctors = await _doctorRepository.GetTotalCountAsync(cancellationToken);
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            var totalIncome = await _billRepository.GetTotalIncomeAsync(cancellationToken);
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            var appointments = await _appointmentRepository.GetTotalCountAsync(cancellationToken);

            var deptStats = departments.Select(d => new DepartmentStatsDto
            {
                DeptName = d.DeptName,
                DoctorCount = d.Doctors.Count,
                PatientCount = 0
            });

            return new AdminDashboardDto
            {
                TotalDoctors = totalDoctors,
                TotalPatients = patients.Count(),
                TotalIncome = totalIncome,
                DepartmentStats = deptStats,
                RecentAppointments = Enumerable.Empty<AppointmentDto>()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving admin dashboard data");
            return new AdminDashboardDto();
        }
    }

    /// <summary>Adds a new staff member</summary>
    public async Task<bool> AddStaffAsync(StaffCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = _mapper.Map<OtherStaff>(dto);
            await _staffRepository.AddAsync(staff, cancellationToken);
            _logger.LogInformation("Staff member added: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {Name}", dto.Name);
            return false;
        }
    }

    /// <summary>Deactivates a staff member</summary>
    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _staffRepository.DeactivateAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member with ID: {Id}", id);
            return false;
        }
    }

    /// <summary>Gets all staff members</summary>
    public async Task<IEnumerable<StaffDto>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            return Enumerable.Empty<StaffDto>();
        }
    }

    /// <summary>Searches staff members</summary>
    public async Task<IEnumerable<StaffDto>> SearchStaffAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.SearchAsync(query, cancellationToken);
            return _mapper.Map<IEnumerable<StaffDto>>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query: {Query}", query);
            return Enumerable.Empty<StaffDto>();
        }
    }

    /// <summary>Gets staff member by ID</summary>
    public async Task<StaffDto?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _staffRepository.GetByIdAsync(id, cancellationToken);
            return staff == null ? null : _mapper.Map<StaffDto>(staff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff member with ID: {Id}", id);
            return null;
        }
    }
}

/// <summary>Appointment service implementation</summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IMapper mapper,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Gets free slots for a doctor</summary>
    public async Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAsync(doctorId, patientId, cancellationToken);
            return slots.Select(a => new FreeSlotDto { SlotId = a.FreeSlot, Timings = a.Timings });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<FreeSlotDto>();
        }
    }

    /// <summary>Books an appointment</summary>
    public async Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = new Domain.Entities.Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                FreeSlot = freeSlot,
                Status = Domain.Enums.AppointmentStatus.Pending,
                AppointmentDate = DateTime.UtcNow
            };
            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            _logger.LogInformation("Appointment booked for patient: {PatientId} with doctor: {DoctorId}", patientId, doctorId);
            return (true, "Appointment request sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient: {PatientId}", patientId);
            return (false, "There was some error booking the appointment.");
        }
    }
}

/// <summary>Department service implementation</summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(
        IDepartmentRepository departmentRepository,
        IMapper mapper,
        ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Gets all departments</summary>
    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var departments = await _departmentRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            return Enumerable.Empty<DepartmentDto>();
        }
    }
}

using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Doctor service implementation</summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBillRepository _billRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        IBillRepository billRepository,
        IMapper mapper,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _billRepository = billRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctor = await _doctorRepository.GetByIdAsync(id, cancellationToken);
            return doctor == null ? null : _mapper.Map<DoctorDto>(doctor);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor with ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all doctors");
            return Enumerable.Empty<DoctorDto>();
        }
    }

    public async Task<IEnumerable<DoctorDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.SearchAsync(query, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching doctors with query: {Query}", query);
            return Enumerable.Empty<DoctorDto>();
        }
    }

    public async Task<IEnumerable<DoctorDto>> GetByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorDto>();
        }
    }

    public async Task<bool> AddDoctorAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            if (await _doctorRepository.EmailExistsAsync(dto.Email, cancellationToken))
                return false;

            var doctor = _mapper.Map<Doctor>(dto);
            await _doctorRepository.AddAsync(doctor, cancellationToken);
            _logger.LogInformation("Doctor added successfully: {Name}", dto.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding doctor: {Name}", dto.Name);
            return false;
        }
    }

    public async Task<bool> DeactivateDoctorAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.DeactivateAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating doctor with ID: {Id}", id);
            return false;
        }
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _doctorRepository.EmailExistsAsync(email, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking email existence: {Email}", email);
            return false;
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetPendingByDoctorAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    public async Task<IEnumerable<AppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<AppointmentDto>();
        }
    }

    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.ApproveAsync(appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            _logger.LogInformation("Prescription updated for appointment: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<IEnumerable<BillDto>> GenerateBillsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bills = await _billRepository.GetByDoctorAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<BillDto>>(bills);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating bills for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<BillDto>();
        }
    }

    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;
            appointment.IsPaid = true;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            _logger.LogInformation("Bill marked as paid for appointment: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;
            appointment.IsPaid = false;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            _logger.LogInformation("Bill marked as unpaid for appointment: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetTodaysByDoctorAsync(doctorId, cancellationToken);
            return _mapper.Map<IEnumerable<PatientHistoryDto>>(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient history for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<PatientHistoryDto>();
        }
    }
}

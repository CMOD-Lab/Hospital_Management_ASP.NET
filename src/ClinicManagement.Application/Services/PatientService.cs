using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Patient service implementation</summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IBillRepository _billRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IBillRepository billRepository,
        IMapper mapper,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _billRepository = billRepository;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>Gets patient by ID</summary>
    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var patient = await _patientRepository.GetByIdAsync(id, cancellationToken);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient with ID: {Id}", id);
            return null;
        }
    }

    /// <summary>Gets all patients</summary>
    public async Task<IEnumerable<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var patients = await _patientRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all patients");
            return Enumerable.Empty<PatientDto>();
        }
    }

    /// <summary>Searches patients by name</summary>
    public async Task<IEnumerable<PatientDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        try
        {
            var patients = await _patientRepository.SearchAsync(query, cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients with query: {Query}", query);
            return Enumerable.Empty<PatientDto>();
        }
    }

    /// <summary>Gets bill history for a patient</summary>
    public async Task<BillHistoryDto> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var bills = await _billRepository.GetByPatientAsync(patientId, cancellationToken);
            var billDtos = _mapper.Map<IEnumerable<BillDto>>(bills);
            return new BillHistoryDto { Count = billDtos.Count(), Bills = billDtos };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient: {PatientId}", patientId);
            return new BillHistoryDto { Count = 0, Bills = Enumerable.Empty<BillDto>() };
        }
    }

    /// <summary>Gets treatment history for a patient</summary>
    public async Task<TreatmentHistoryDto> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointments = await _appointmentRepository.GetByPatientAsync(patientId, cancellationToken);
            var records = _mapper.Map<IEnumerable<TreatmentRecordDto>>(appointments);
            return new TreatmentHistoryDto { Count = records.Count(), Records = records };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient: {PatientId}", patientId);
            return new TreatmentHistoryDto { Count = 0, Records = Enumerable.Empty<TreatmentRecordDto>() };
        }
    }

    /// <summary>Gets current appointment for a patient</summary>
    public async Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new CurrentAppointmentDto
            {
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Timings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets notifications for a patient</summary>
    public async Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetCurrentByPatientAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new NotificationDto
            {
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Timings,
                Count = 1
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Gets pending feedback for a patient</summary>
    public async Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            var appointment = await _appointmentRepository.GetPendingFeedbackAsync(patientId, cancellationToken);
            if (appointment == null) return null;
            return new PendingFeedbackDto
            {
                AppointmentId = appointment.AppointmentId,
                DoctorName = appointment.Doctor?.Name ?? string.Empty,
                Timings = appointment.Timings
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>Submits feedback for an appointment</summary>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.StoreFeedbackAsync(appointmentId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }
}

using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CareTrack.Application.Services;

/// <summary>
/// Service implementation for patient operations.
/// </summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        IDoctorRepository doctorRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _doctorRepository = doctorRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets patient information by ID.
    /// </summary>
    public async Task<Patient?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient info for ID: {PatientId}", patientId);
            return await _patientRepository.GetByIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient info for ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets bill history for a patient.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bill history for patient ID: {PatientId}", patientId);
            return await _appointmentRepository.GetBillHistoryByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Gets the current appointment for a patient.
    /// </summary>
    public async Task<Appointment?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving current appointment for patient ID: {PatientId}", patientId);
            return await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets treatment history for a patient.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving treatment history for patient ID: {PatientId}", patientId);
            return await _appointmentRepository.GetTreatmentHistoryByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Gets all departments.
    /// </summary>
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

    /// <summary>
    /// Gets doctors by department name.
    /// </summary>
    public async Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department: {DeptName}", deptName);
            return await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<Doctor>();
        }
    }

    /// <summary>
    /// Gets doctor profile by ID.
    /// </summary>
    public async Task<Doctor?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor profile for ID: {DoctorId}", doctorId);
            return await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor profile for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets available appointment slots for a doctor.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving free slots for doctor ID: {DoctorId}, patient ID: {PatientId}", doctorId, patientId);
            return await _appointmentRepository.GetFreeSlotsByDoctorAndPatientAsync(doctorId, patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Books an appointment for a patient.
    /// </summary>
    public async Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlotId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient ID: {PatientId} with doctor ID: {DoctorId}", patientId, doctorId);

            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Date = DateTime.Now,
                AppointmentStatus = 2, // Pending
                DoctorNotification = 2, // Unseen
                PatientNotification = 2, // Unseen
                FeedbackStatus = 2 // Pending
            };

            await _appointmentRepository.AddAsync(appointment, cancellationToken);

            _logger.LogInformation("Appointment booked successfully");
            return (true, "Appointment request sent successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient ID: {PatientId}", patientId);
            return (false, "An error occurred while booking the appointment.");
        }
    }

    /// <summary>
    /// Gets notification for a patient.
    /// </summary>
    public async Task<Appointment?> GetNotificationAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving notification for patient ID: {PatientId}", patientId);
            return await _appointmentRepository.GetNotificationByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Gets pending feedback for a patient.
    /// </summary>
    public async Task<Appointment?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending feedback for patient ID: {PatientId}", patientId);
            return await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <summary>
    /// Submits feedback for an appointment.
    /// </summary>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found for ID: {AppointmentId}", appointmentId);
                return false;
            }

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            _logger.LogInformation("Feedback submitted successfully for appointment ID: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }
}

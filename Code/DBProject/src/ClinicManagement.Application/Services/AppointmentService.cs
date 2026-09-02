using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for appointment operations.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    public async Task<Appointment?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving appointment with ID: {AppointmentId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all appointments");
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending appointments for doctor: {DoctorId}", doctorId);
            return await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving today's appointments for doctor: {DoctorId}", doctorId);
            return await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetCurrentAppointmentByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetBillHistoryByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetTreatmentHistoryByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient: {PatientId}", patientId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<Appointment?> GetNotificationByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetNotificationByPatientIdAsync(patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notification for patient: {PatientId}", patientId);
            return null;
        }
    }

    public async Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetFreeSlotsByDoctorAndPatientAsync(doctorId, patientId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    public async Task<bool> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Booking appointment for patient: {PatientId} with doctor: {DoctorId}", patientId, doctorId);
            var appointment = new Appointment
            {
                DoctorId = doctorId,
                PatientId = patientId,
                Status = AppointmentStatus.Pending,
                AppointmentDate = DateTime.Today,
                DoctorNotification = 2,
                PatientNotification = 2,
                FeedbackStatus = 2
            };
            await _appointmentRepository.AddAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient: {PatientId}", patientId);
            return false;
        }
    }

    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Approved;
            appointment.PatientNotification = 2;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
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
            _logger.LogInformation("Deleting appointment: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
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
            _logger.LogInformation("Updating prescription for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            appointment.Status = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Paid;
            appointment.Status = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
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
            _logger.LogInformation("Marking bill as unpaid for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.BillStatus = BillStatus.Unpaid;
            appointment.Status = AppointmentStatus.Completed;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Storing feedback for appointment: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing feedback for appointment: {AppointmentId}", appointmentId);
            return false;
        }
    }

    public async Task<IEnumerable<Appointment>> GetHistoryByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _appointmentRepository.GetHistoryByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving history for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }
}

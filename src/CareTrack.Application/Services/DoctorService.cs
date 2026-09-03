using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CareTrack.Application.Services;

/// <summary>
/// Service implementation for doctor operations.
/// </summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _logger = logger;
    }

    /// <summary>
    /// Gets doctor information by ID.
    /// </summary>
    public async Task<Doctor?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor info for ID: {DoctorId}", doctorId);
            return await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor info for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <summary>
    /// Gets pending appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            return await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Gets today's appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            return await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Approves a pending appointment.
    /// </summary>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null)
            {
                _logger.LogWarning("Appointment not found for ID: {AppointmentId}", appointmentId);
                return false;
            }

            appointment.AppointmentStatus = 1; // Approved
            appointment.PatientNotification = 2; // Unseen - notify patient
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            _logger.LogInformation("Appointment approved successfully: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Rejects a pending appointment.
    /// </summary>
    public async Task<bool> RejectAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Rejecting appointment ID: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            _logger.LogInformation("Appointment rejected/deleted: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Updates prescription for an appointment.
    /// </summary>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId)
            {
                _logger.LogWarning("Appointment not found or unauthorized for ID: {AppointmentId}", appointmentId);
                return false;
            }

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            appointment.AppointmentStatus = 3; // Completed

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            _logger.LogInformation("Prescription updated successfully for appointment ID: {AppointmentId}", appointmentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets billable appointments for a doctor.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetBillableAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving billable appointments for doctor ID: {DoctorId}", doctorId);
            return await _appointmentRepository.GetBillableByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving billable appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }

    /// <summary>
    /// Marks a bill as paid.
    /// </summary>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId)
            {
                return false;
            }

            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = 3; // Completed
            appointment.FeedbackStatus = 2; // Pending feedback
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Marks a bill as unpaid.
    /// </summary>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);

            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId)
            {
                return false;
            }

            appointment.BillStatus = "Unpaid";
            appointment.AppointmentStatus = 3; // Completed
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <summary>
    /// Gets patient history for a doctor.
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient history for doctor ID: {DoctorId}", doctorId);
            return await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient history for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<Appointment>();
        }
    }
}

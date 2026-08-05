using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Handles all doctor-related business operations.</summary>
public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(
        IDoctorRepository doctorRepository,
        IAppointmentRepository appointmentRepository,
        IDepartmentRepository departmentRepository,
        ILogger<DoctorService> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<DoctorInfoDto?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor info for ID: {DoctorId}", doctorId);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            return new DoctorInfoDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Address = doctor.Address,
                Department = doctor.Department?.DeptName,
                Specialization = doctor.Specialization,
                Qualification = doctor.Qualification,
                ChargesPerVisit = doctor.ChargesPerVisit,
                ReputeIndex = doctor.ReputeIndex,
                PatientsTreated = doctor.PatientsTreated
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor info for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<PendingAppointmentDto>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetPendingByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new PendingAppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Status = a.AppointmentStatus.HasValue ? a.AppointmentStatus.Value.ToString() : "Pending"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<PendingAppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Approving appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.AppointmentStatus = 1; // Approved
            appointment.DoctorNotification = 1; // Seen
            appointment.PatientNotification = 2; // Unseen (notify patient)
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting appointment ID: {AppointmentId}", appointmentId);
            await _appointmentRepository.DeleteAsync(appointmentId, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TodayAppointmentDto>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetTodaysByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new TodayAppointmentDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving today's appointments for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<TodayAppointmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Updating prescription for appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.Disease = disease;
            appointment.Progress = progress;
            appointment.Prescription = prescription;
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating prescription for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BillDto>> GetBillsAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bills for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetBillsByDoctorIdAsync(doctorId, cancellationToken);

            return appointments.Select(a => new BillDto
            {
                AppointId = a.AppointId,
                PatientName = a.Patient?.Name,
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<BillDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

            appointment.BillStatus = "Paid";
            appointment.AppointmentStatus = 3; // Completed
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            // Update doctor's patients treated count
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor != null)
            {
                doctor.PatientsTreated++;
                await _doctorRepository.UpdateAsync(doctor, cancellationToken);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking bill as paid for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Marking bill as unpaid for appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null || appointment.DoctorId != doctorId) return false;

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

    /// <inheritdoc/>
    public async Task<IEnumerable<PatientHistoryDto>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient history for doctor ID: {DoctorId}", doctorId);
            var appointments = await _appointmentRepository.GetByDoctorIdAsync(doctorId, cancellationToken);

            return appointments
                .Where(a => a.AppointmentStatus == 3) // Completed
                .Select(a => new PatientHistoryDto
                {
                    AppointId = a.AppointId,
                    PatientName = a.Patient?.Name,
                    Date = a.Date,
                    Disease = a.Disease,
                    Progress = a.Progress,
                    Prescription = a.Prescription,
                    BillStatus = a.BillStatus
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient history for doctor ID: {DoctorId}", doctorId);
            return Enumerable.Empty<PatientHistoryDto>();
        }
    }
}

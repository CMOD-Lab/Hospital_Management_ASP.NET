using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Handles all patient-related business operations.</summary>
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILoginRepository _loginRepository;
    private readonly ILogger<PatientService> _logger;

    public PatientService(
        IPatientRepository patientRepository,
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IDepartmentRepository departmentRepository,
        ILoginRepository loginRepository,
        ILogger<PatientService> logger)
    {
        _patientRepository = patientRepository;
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _departmentRepository = departmentRepository;
        _loginRepository = loginRepository;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<PatientInfoDto?> GetPatientInfoAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving patient info for ID: {PatientId}", patientId);
            var patient = await _patientRepository.GetByIdAsync(patientId, cancellationToken);
            if (patient == null) return null;

            int age = DateTime.Today.Year - patient.BirthDate.Year;
            if (patient.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new PatientInfoDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                Phone = patient.Phone,
                Address = patient.Address,
                BirthDate = patient.BirthDate.ToString("yyyy-MM-dd"),
                Age = age,
                Gender = patient.Gender.ToString()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving patient info for ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BillHistoryDto>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving bill history for patient ID: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetBillHistoryByPatientIdAsync(patientId, cancellationToken);

            return appointments.Select(a => new BillHistoryDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name,
                Date = a.Date,
                BillAmount = a.BillAmount,
                BillStatus = a.BillStatus
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bill history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<BillHistoryDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<CurrentAppointmentDto?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving current appointment for patient ID: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetCurrentByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new CurrentAppointmentDto
            {
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : "N/A"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current appointment for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<TreatmentHistoryDto>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving treatment history for patient ID: {PatientId}", patientId);
            var appointments = await _appointmentRepository.GetTreatmentHistoryByPatientIdAsync(patientId, cancellationToken);

            return appointments.Select(a => new TreatmentHistoryDto
            {
                AppointId = a.AppointId,
                DoctorName = a.Doctor?.Name,
                Date = a.Date,
                Disease = a.Disease,
                Progress = a.Progress,
                Prescription = a.Prescription
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving treatment history for patient ID: {PatientId}", patientId);
            return Enumerable.Empty<TreatmentHistoryDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DepartmentDto>> GetDepartmentInfoAsync(CancellationToken cancellationToken = default)
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
            _logger.LogError(ex, "Error retrieving department info");
            return Enumerable.Empty<DepartmentDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DoctorListItemDto>> GetDoctorsByDepartmentAsync(string deptName, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctors for department: {DeptName}", deptName);
            var doctors = await _doctorRepository.GetByDepartmentAsync(deptName, cancellationToken);

            return doctors.Select(d => new DoctorListItemDto
            {
                DoctorId = d.DoctorId,
                Name = d.Name,
                Department = d.Department?.DeptName ?? deptName
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctors for department: {DeptName}", deptName);
            return Enumerable.Empty<DoctorListItemDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<DoctorProfileDto?> GetDoctorProfileAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving doctor profile for ID: {DoctorId}", doctorId);
            var doctor = await _doctorRepository.GetByIdAsync(doctorId, cancellationToken);
            if (doctor == null) return null;

            int age = DateTime.Today.Year - doctor.BirthDate.Year;
            if (doctor.BirthDate.Date > DateTime.Today.AddYears(-age)) age--;

            return new DoctorProfileDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Phone = doctor.Phone,
                Gender = doctor.Gender.ToString(),
                ChargesPerVisit = (float)doctor.ChargesPerVisit,
                ReputeIndex = (float)(doctor.ReputeIndex ?? 0),
                PatientsTreated = doctor.PatientsTreated,
                Qualification = doctor.Qualification,
                Specialization = doctor.Specialization,
                WorkExperience = doctor.WorkExperience ?? 0,
                Age = age
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving doctor profile for ID: {DoctorId}", doctorId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<FreeSlotDto>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving free slots for doctor: {DoctorId}, patient: {PatientId}", doctorId, patientId);
            var slots = await _appointmentRepository.GetFreeSlotsByDoctorAndPatientAsync(doctorId, patientId, cancellationToken);

            return slots.Select(s => new FreeSlotDto
            {
                SlotId = s.AppointId,
                SlotTime = s.Date.HasValue ? s.Date.Value.ToString("yyyy-MM-dd HH:mm") : "N/A"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving free slots");
            return Enumerable.Empty<FreeSlotDto>();
        }
    }

    /// <inheritdoc/>
    public async Task<AppointmentResultDto> InsertAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Inserting appointment for doctor: {DoctorId}, patient: {PatientId}, slot: {FreeSlot}", doctorId, patientId, freeSlot);

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
            return new AppointmentResultDto { Success = true, Message = "Appointment request sent successfully." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error inserting appointment");
            return new AppointmentResultDto { Success = false, Message = "An error occurred. Please try again." };
        }
    }

    /// <inheritdoc/>
    public async Task<NotificationDto?> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving notifications for patient ID: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetNotificationByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new NotificationDto
            {
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : "N/A"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving notifications for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<PendingFeedbackDto?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving pending feedback for patient ID: {PatientId}", patientId);
            var appointment = await _appointmentRepository.GetPendingFeedbackByPatientIdAsync(patientId, cancellationToken);
            if (appointment == null) return null;

            return new PendingFeedbackDto
            {
                AppointmentId = appointment.AppointId,
                DoctorName = appointment.Doctor?.Name ?? "Unknown",
                Timings = appointment.Date.HasValue ? appointment.Date.Value.ToString("yyyy-MM-dd HH:mm") : "N/A"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending feedback for patient ID: {PatientId}", patientId);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Submitting feedback for appointment ID: {AppointmentId}", appointmentId);
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
            if (appointment == null) return false;

            appointment.FeedbackStatus = 1; // Given
            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error submitting feedback for appointment ID: {AppointmentId}", appointmentId);
            return false;
        }
    }
}

using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for appointment operations.
/// </summary>
public interface IAppointmentService
{
    Task<Appointment?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPendingAppointmentsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTodaysAppointmentsByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetCurrentAppointmentByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetBillHistoryByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTreatmentHistoryByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetPendingFeedbackByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetNotificationByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<bool> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default);
    Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetHistoryByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
}

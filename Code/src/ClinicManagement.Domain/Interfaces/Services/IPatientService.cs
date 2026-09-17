using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for patient operations.</summary>
public interface IPatientService
{
    Task<Patient?> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetAllPatientsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetBillHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TreatmentHistory>> GetTreatmentHistoryAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetCurrentAppointmentAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetNotificationsAsync(int patientId, CancellationToken cancellationToken = default);
    Task<(bool HasPending, Appointment? PendingAppointment)> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> SubmitFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
}

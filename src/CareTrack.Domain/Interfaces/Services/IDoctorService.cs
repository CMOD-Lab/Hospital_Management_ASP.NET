using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Interfaces.Services;

/// <summary>
/// Service interface for doctor operations.
/// </summary>
public interface IDoctorService
{
    Task<Doctor?> GetDoctorInfoAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPendingAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTodaysAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> ApproveAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> RejectAppointmentAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> UpdatePrescriptionAsync(int doctorId, int appointmentId, string disease, string progress, string prescription, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetBillableAppointmentsAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillPaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> MarkBillUnpaidAsync(int doctorId, int appointmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPatientHistoryAsync(int doctorId, CancellationToken cancellationToken = default);
}

using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Appointment repository interface</summary>
public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetPendingByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetTodaysByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetCurrentByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsByDoctorAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<Appointment> AddAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken = default);
    Task<bool> ApproveAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<Appointment?> GetPendingFeedbackAsync(int patientId, CancellationToken cancellationToken = default);
    Task<bool> StoreFeedbackAsync(int appointmentId, CancellationToken cancellationToken = default);
    Task<int> GetTotalCountAsync(CancellationToken cancellationToken = default);
}

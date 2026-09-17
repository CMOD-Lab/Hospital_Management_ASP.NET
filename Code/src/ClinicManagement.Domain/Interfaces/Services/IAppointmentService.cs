using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for appointment operations.</summary>
public interface IAppointmentService
{
    Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetFreeSlotsAsync(int doctorId, int patientId, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> BookAppointmentAsync(int doctorId, int patientId, int freeSlot, CancellationToken cancellationToken = default);
}

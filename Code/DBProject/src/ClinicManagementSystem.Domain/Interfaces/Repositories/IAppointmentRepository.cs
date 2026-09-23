using ClinicManagementSystem.Domain.Entities;

namespace ClinicManagementSystem.Domain.Interfaces.Repositories;

public interface IAppointmentRepository
{
    Task<IReadOnlyList<Appointment>> GetAllAsync(CancellationToken cancellationToken);
    Task<Appointment?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<Appointment> AddAsync(Appointment entity, CancellationToken cancellationToken);
    Task UpdateAsync(Appointment entity, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Appointment>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}

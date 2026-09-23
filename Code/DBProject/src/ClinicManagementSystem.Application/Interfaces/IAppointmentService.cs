using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Application.Interfaces;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken);
    Task UpdateAsync(int id, AppointmentUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppointmentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}

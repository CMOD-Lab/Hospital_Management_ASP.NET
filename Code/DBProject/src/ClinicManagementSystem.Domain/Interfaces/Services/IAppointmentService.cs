using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Domain.Interfaces.Services;

public interface IAppointmentService
{
    Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, AppointmentUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AppointmentDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
    Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
}

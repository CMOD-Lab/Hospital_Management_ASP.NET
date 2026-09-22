using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Domain.Interfaces.Services;

public interface IDoctorService
{
    Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<DoctorDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DoctorDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}

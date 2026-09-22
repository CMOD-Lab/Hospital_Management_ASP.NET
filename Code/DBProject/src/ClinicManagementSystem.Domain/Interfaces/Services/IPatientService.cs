using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Domain.Interfaces.Services;

public interface IPatientService
{
    Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PatientDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PatientDto>> SearchAsync(string query, CancellationToken cancellationToken = default);
}

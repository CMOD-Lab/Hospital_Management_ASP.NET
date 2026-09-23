using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Application.Interfaces;

public interface IPatientService
{
    Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<PatientDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken);
    Task UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PatientDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}

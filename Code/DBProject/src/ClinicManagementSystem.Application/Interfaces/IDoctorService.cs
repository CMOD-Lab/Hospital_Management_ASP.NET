using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Application.Interfaces;

public interface IDoctorService
{
    Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<DoctorDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken);
    Task UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DoctorDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}

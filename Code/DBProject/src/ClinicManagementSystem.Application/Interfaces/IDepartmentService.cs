using ClinicManagementSystem.Application.DTOs;

namespace ClinicManagementSystem.Application.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto, CancellationToken cancellationToken);
    Task UpdateAsync(int id, DepartmentUpdateDto dto, CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<DepartmentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken);
}

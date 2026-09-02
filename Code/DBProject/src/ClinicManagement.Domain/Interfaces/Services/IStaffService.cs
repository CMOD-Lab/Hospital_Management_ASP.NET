using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for staff operations.
/// </summary>
public interface IStaffService
{
    Task<OtherStaff?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> GetAllStaffAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> SearchStaffAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(OtherStaff staff, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default);
}

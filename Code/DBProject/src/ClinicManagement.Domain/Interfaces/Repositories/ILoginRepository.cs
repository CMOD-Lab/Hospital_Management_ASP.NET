using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for LoginEntry operations.
/// </summary>
public interface ILoginRepository
{
    Task<LoginEntry?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<LoginEntry?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<LoginEntry> AddAsync(LoginEntry loginEntry, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Interfaces.Repositories;

/// <summary>
/// Repository interface for authentication data access.
/// </summary>
public interface IAuthRepository
{
    Task<LoginTable?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<LoginTable> CreateLoginAsync(LoginTable login, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
    Task<LoginTable?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
}

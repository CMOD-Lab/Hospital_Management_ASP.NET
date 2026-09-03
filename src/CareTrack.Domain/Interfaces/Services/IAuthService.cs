using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Interfaces.Services;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    Task<(bool Success, int UserId, int UserType, string Message)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Success, int PatientId, string Message)> RegisterPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken cancellationToken = default);
}

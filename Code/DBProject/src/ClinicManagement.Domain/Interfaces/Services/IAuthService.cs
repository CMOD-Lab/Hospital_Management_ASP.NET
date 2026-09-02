using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for authentication operations.
/// </summary>
public interface IAuthService
{
    Task<(bool Success, int UserId, UserType UserType)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Success, int UserId, string Message)> RegisterPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken cancellationToken = default);
}

using ClinicManagement.Domain.Enums;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for authentication operations.</summary>
public interface IAuthService
{
    /// <summary>Validates login credentials and returns user type and ID.</summary>
    Task<(int Status, UserType UserType, int UserId)> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default);

    /// <summary>Registers a new patient.</summary>
    Task<(int Status, int PatientId)> RegisterPatientAsync(string name, string birthDate, string email, string password, string phone, string gender, string address, CancellationToken cancellationToken = default);
}

using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for authentication operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly ILoginRepository _loginRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ILoginRepository loginRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _loginRepository = loginRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates user login credentials.
    /// </summary>
    public async Task<(bool Success, int UserId, UserType UserType)> ValidateLoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email: {Email}", email);
            var loginEntry = await _loginRepository.ValidateLoginAsync(email, password, cancellationToken);

            if (loginEntry == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", email);
                return (false, 0, UserType.Patient);
            }

            _logger.LogInformation("Login successful for user ID: {UserId}", loginEntry.LoginId);
            return (true, loginEntry.LoginId, (UserType)loginEntry.Type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login validation for email: {Email}", email);
            return (false, 0, UserType.Patient);
        }
    }

    /// <summary>
    /// Registers a new patient.
    /// </summary>
    public async Task<(bool Success, int UserId, string Message)> RegisterPatientAsync(
        string name, string birthDate, string email, string password,
        string phone, string gender, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new patient with email: {Email}", email);

            if (await _loginRepository.EmailExistsAsync(email, cancellationToken))
            {
                _logger.LogWarning("Registration failed - email already exists: {Email}", email);
                return (false, 0, "Email already exists.");
            }

            var loginEntry = new LoginEntry
            {
                Email = email,
                Password = password,
                Type = (int)UserType.Patient
            };
            var createdLogin = await _loginRepository.AddAsync(loginEntry, cancellationToken);

            var patient = new Patient
            {
                PatientId = createdLogin.LoginId,
                Name = name,
                Phone = phone,
                Address = address,
                BirthDate = DateTime.Parse(birthDate),
                Gender = string.IsNullOrEmpty(gender) ? 'M' : gender[0]
            };
            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient registered successfully with ID: {PatientId}", createdLogin.LoginId);
            return (true, createdLogin.LoginId, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient with email: {Email}", email);
            return (false, 0, "Registration failed due to an error.");
        }
    }
}

using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace CareTrack.Application.Services;

/// <summary>
/// Service implementation for authentication operations.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IAuthRepository authRepository,
        IPatientRepository patientRepository,
        ILogger<AuthService> logger)
    {
        _authRepository = authRepository;
        _patientRepository = patientRepository;
        _logger = logger;
    }

    /// <summary>
    /// Validates user login credentials.
    /// </summary>
    public async Task<(bool Success, int UserId, int UserType, string Message)> ValidateLoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Validating login for email: {Email}", email);

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return (false, 0, 0, "Email and password are required.");
            }

            var login = await _authRepository.ValidateLoginAsync(email, password, cancellationToken);

            if (login == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", email);
                return (false, 0, 0, "Invalid email or password.");
            }

            _logger.LogInformation("Login successful for user ID: {UserId}", login.LoginId);
            return (true, login.LoginId, login.Type, "Login successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login validation for email: {Email}", email);
            return (false, 0, 0, "An error occurred during login. Please try again.");
        }
    }

    /// <summary>
    /// Registers a new patient in the system.
    /// </summary>
    public async Task<(bool Success, int PatientId, string Message)> RegisterPatientAsync(
        string name, string birthDate, string email, string password,
        string phone, string gender, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Registering new patient with email: {Email}", email);

            if (await _authRepository.EmailExistsAsync(email, cancellationToken))
            {
                return (false, 0, "Email already exists. Please choose a different one.");
            }

            if (!DateTime.TryParse(birthDate, out var parsedDate))
            {
                return (false, 0, "Invalid birth date format.");
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = email,
                Password = password,
                Type = 1 // Patient
            };

            var createdLogin = await _authRepository.CreateLoginAsync(login, cancellationToken);

            // Create patient entry
            var patient = new Patient
            {
                PatientId = createdLogin.LoginId,
                Name = name,
                BirthDate = parsedDate,
                Phone = phone,
                Gender = gender,
                Address = address
            };

            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient registered successfully with ID: {PatientId}", createdLogin.LoginId);
            return (true, createdLogin.LoginId, "Registration successful.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering patient with email: {Email}", email);
            return (false, 0, "An error occurred during registration. Please try again.");
        }
    }
}

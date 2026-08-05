using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Handles authentication: login and patient sign-up.</summary>
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

    /// <inheritdoc/>
    public async Task<LoginResultDto> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", email);

            var login = await _loginRepository.ValidateLoginAsync(email, password, cancellationToken);

            if (login == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", email);
                return new LoginResultDto { Success = false, ErrorMessage = "Invalid email or password." };
            }

            _logger.LogInformation("Login successful for user ID: {UserId}, Type: {UserType}", login.LoginId, login.Type);
            return new LoginResultDto
            {
                Success = true,
                UserId = login.LoginId,
                UserType = login.Type
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", email);
            return new LoginResultDto { Success = false, ErrorMessage = "An error occurred. Please try again." };
        }
    }

    /// <inheritdoc/>
    public async Task<SignUpResultDto> SignUpPatientAsync(PatientSignUpDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Patient sign-up attempt for email: {Email}", dto.Email);

            // Check if email already exists
            bool emailExists = await _loginRepository.EmailExistsAsync(dto.Email, cancellationToken);
            if (emailExists)
            {
                return new SignUpResultDto { Success = false, ErrorMessage = "Email already exists. Please choose a different one." };
            }

            // Create login entry
            var login = new LoginTable
            {
                Email = dto.Email,
                Password = dto.Password,
                Type = 1 // Patient
            };

            int loginId = await _loginRepository.AddAsync(login, cancellationToken);

            // Parse birth date
            if (!DateTime.TryParse(dto.BirthDate, out DateTime birthDate))
            {
                return new SignUpResultDto { Success = false, ErrorMessage = "Invalid birth date format." };
            }

            // Create patient entry
            var patient = new Patient
            {
                PatientId = loginId,
                Name = dto.Name,
                BirthDate = birthDate,
                Phone = dto.PhoneNo,
                Gender = string.IsNullOrEmpty(dto.Gender) ? 'M' : dto.Gender[0],
                Address = dto.Address
            };

            await _patientRepository.AddAsync(patient, cancellationToken);

            _logger.LogInformation("Patient sign-up successful. PatientId: {PatientId}", loginId);
            return new SignUpResultDto { Success = true, PatientId = loginId };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient sign-up for email: {Email}", dto.Email);
            return new SignUpResultDto { Success = false, ErrorMessage = "An error occurred. Please try again." };
        }
    }
}

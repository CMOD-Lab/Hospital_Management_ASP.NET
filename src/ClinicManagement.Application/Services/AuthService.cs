using AutoMapper;
using ClinicManagement.Application.DTOs;
using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Enums;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>Authentication service implementation</summary>
public class AuthService : IAuthService
{
    private readonly IPatientRepository _patientRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IPatientRepository patientRepository,
        IDoctorRepository doctorRepository,
        ILogger<AuthService> logger)
    {
        _patientRepository = patientRepository;
        _doctorRepository = doctorRepository;
        _logger = logger;
    }

    /// <summary>Validates login credentials and returns user type</summary>
    public async Task<(bool Success, int UserId, UserType UserType, string Message)> LoginAsync(
        string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Login attempt for email: {Email}", email);

            // Check patient
            var patient = await _patientRepository.GetByEmailAsync(email, cancellationToken);
            if (patient != null)
            {
                if (patient.Password == password)
                {
                    _logger.LogInformation("Patient login successful for ID: {Id}", patient.PatientId);
                    return (true, patient.PatientId, UserType.Patient, "Login successful");
                }
                return (false, 0, UserType.Patient, "Incorrect Password. Try Again!");
            }

            // Check doctor
            var doctor = await _doctorRepository.GetByEmailAsync(email, cancellationToken);
            if (doctor != null)
            {
                if (doctor.Password == password)
                {
                    _logger.LogInformation("Doctor login successful for ID: {Id}", doctor.DoctorId);
                    return (true, doctor.DoctorId, UserType.Doctor, "Login successful");
                }
                return (false, 0, UserType.Doctor, "Incorrect Password. Try Again!");
            }

            // Check admin (hardcoded admin for legacy compatibility)
            if (email == "admin@clinic.com" && password == "admin123")
            {
                return (true, 1, UserType.Admin, "Login successful");
            }

            return (false, 0, UserType.Patient, "Email not found. Try Again!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for email: {Email}", email);
            return (false, 0, UserType.Patient, "There was some error. Try Again!");
        }
    }

    /// <summary>Registers a new patient</summary>
    public async Task<(bool Success, int PatientId, string Message)> RegisterPatientAsync(
        string name, string birthDate, string email, string password,
        string phone, string gender, string address, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Patient registration attempt for email: {Email}", email);

            if (await _patientRepository.EmailExistsAsync(email, cancellationToken))
            {
                return (false, 0, "Email already exists. Please choose a different one.");
            }

            var patient = new Patient
            {
                Name = name,
                Email = email,
                Password = password,
                Phone = phone,
                Gender = gender,
                Address = address,
                BirthDate = DateTime.TryParse(birthDate, out var bd) ? bd : DateTime.UtcNow,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var created = await _patientRepository.AddAsync(patient, cancellationToken);
            _logger.LogInformation("Patient registered successfully with ID: {Id}", created.PatientId);
            return (true, created.PatientId, "Registration successful");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during patient registration for email: {Email}", email);
            return (false, 0, "There was some error. Try again!");
        }
    }
}

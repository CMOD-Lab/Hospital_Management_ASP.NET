namespace ClinicManagementSystem.Application.DTOs;

public sealed record PatientDto(
    int Id,
    string Name,
    string Email,
    string PhoneNumber,
    DateTime BirthDate,
    string Gender,
    string Address);

public sealed record PatientCreateDto(
    string Name,
    string Email,
    string PhoneNumber,
    DateTime BirthDate,
    string Gender,
    string Address);

public sealed record PatientUpdateDto(
    string Name,
    string Email,
    string PhoneNumber,
    DateTime BirthDate,
    string Gender,
    string Address);

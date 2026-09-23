namespace ClinicManagementSystem.Application.DTOs;

public sealed record DoctorDto(
    int Id,
    string Name,
    string Email,
    string PhoneNumber,
    string Gender,
    string Qualification,
    string Specialization,
    string Address,
    decimal ChargesPerVisit,
    int ExperienceYears,
    int DepartmentId,
    string? DepartmentName);

public sealed record DoctorCreateDto(
    string Name,
    string Email,
    string PhoneNumber,
    string Gender,
    string Qualification,
    string Specialization,
    string Address,
    decimal ChargesPerVisit,
    int ExperienceYears,
    int DepartmentId);

public sealed record DoctorUpdateDto(
    string Name,
    string Email,
    string PhoneNumber,
    string Gender,
    string Qualification,
    string Specialization,
    string Address,
    decimal ChargesPerVisit,
    int ExperienceYears,
    int DepartmentId);

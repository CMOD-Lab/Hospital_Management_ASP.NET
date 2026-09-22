namespace ClinicManagementSystem.Application.DTOs;

public record PatientDto(int Id, string Name, string Email, string PhoneNumber, string Gender, string Address, DateTime BirthDate);
public record PatientCreateDto(string Name, string Email, string PhoneNumber, string Gender, string Address, DateTime BirthDate);
public record PatientUpdateDto(string Name, string Email, string PhoneNumber, string Gender, string Address, DateTime BirthDate, bool IsActive);

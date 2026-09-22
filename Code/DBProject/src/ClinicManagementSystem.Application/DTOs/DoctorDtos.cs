namespace ClinicManagementSystem.Application.DTOs;

public record DoctorDto(int Id, string Name, string Email, string PhoneNumber, string Qualification, string Specialization, int YearsOfExperience, decimal ChargesPerVisit, int DepartmentId);
public record DoctorCreateDto(string Name, string Email, string PhoneNumber, string Gender, string Address, DateTime BirthDate, string Qualification, string Specialization, int YearsOfExperience, decimal Salary, decimal ChargesPerVisit, int DepartmentId);
public record DoctorUpdateDto(string Name, string Email, string PhoneNumber, string Gender, string Address, DateTime BirthDate, string Qualification, string Specialization, int YearsOfExperience, decimal Salary, decimal ChargesPerVisit, int DepartmentId, bool IsActive);

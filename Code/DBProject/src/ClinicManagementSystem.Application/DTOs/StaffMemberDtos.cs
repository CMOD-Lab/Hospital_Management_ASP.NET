namespace ClinicManagementSystem.Application.DTOs;

public record StaffMemberDto(int Id, string Name, string PhoneNumber, string Gender, string Address, DateTime BirthDate, string Qualification, string Designation, decimal Salary);
public record StaffMemberCreateDto(string Name, string PhoneNumber, string Gender, string Address, DateTime BirthDate, string Qualification, string Designation, decimal Salary);
public record StaffMemberUpdateDto(string Name, string PhoneNumber, string Gender, string Address, DateTime BirthDate, string Qualification, string Designation, decimal Salary, bool IsActive);

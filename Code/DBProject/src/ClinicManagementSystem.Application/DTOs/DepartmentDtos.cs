namespace ClinicManagementSystem.Application.DTOs;

public sealed record DepartmentDto(int Id, string Name, string? Description);

public sealed record DepartmentCreateDto(string Name, string? Description);

public sealed record DepartmentUpdateDto(string Name, string? Description);

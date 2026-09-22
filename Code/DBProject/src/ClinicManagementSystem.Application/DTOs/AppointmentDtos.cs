namespace ClinicManagementSystem.Application.DTOs;

public record AppointmentDto(int Id, int PatientId, int DoctorId, DateTime ScheduledAt, string Status, string Notes);
public record AppointmentCreateDto(int PatientId, int DoctorId, DateTime ScheduledAt, string Notes);
public record AppointmentUpdateDto(int PatientId, int DoctorId, DateTime ScheduledAt, string Status, string Notes, bool IsActive);

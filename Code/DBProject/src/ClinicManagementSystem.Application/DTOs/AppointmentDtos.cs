namespace ClinicManagementSystem.Application.DTOs;

public sealed record AppointmentDto(
    int Id,
    string Name,
    int PatientId,
    string? PatientName,
    int DoctorId,
    string? DoctorName,
    DateTime ScheduledAt,
    string Status,
    string? Prescription,
    string? ProgressNotes,
    string? Disease);

public sealed record AppointmentCreateDto(
    string Name,
    int PatientId,
    int DoctorId,
    DateTime ScheduledAt,
    string Status,
    string? Prescription,
    string? ProgressNotes,
    string? Disease);

public sealed record AppointmentUpdateDto(
    string Name,
    int PatientId,
    int DoctorId,
    DateTime ScheduledAt,
    string Status,
    string? Prescription,
    string? ProgressNotes,
    string? Disease);

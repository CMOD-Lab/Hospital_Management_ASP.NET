using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IStaffMemberRepository _staffRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IDoctorRepository doctorRepository,
        IPatientRepository patientRepository,
        IStaffMemberRepository staffRepository,
        IMapper mapper,
        ILogger<AppointmentService> logger)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        _staffRepository = staffRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _appointmentRepository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<AppointmentDto>>(entities);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _appointmentRepository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<AppointmentDto>(entity);
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken = default)
    {
        if (!await _doctorRepository.ExistsAsync(dto.DoctorId, cancellationToken) || !await _patientRepository.ExistsAsync(dto.PatientId, cancellationToken))
        {
            throw new InvalidOperationException("Doctor or patient not found.");
        }

        var entity = _mapper.Map<Appointment>(dto);
        var created = await _appointmentRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Appointment {AppointmentId} created", created.Id);
        return _mapper.Map<AppointmentDto>(created);
    }

    public async Task UpdateAsync(int id, AppointmentUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _appointmentRepository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Appointment not found.");
        _mapper.Map(dto, existing);
        await _appointmentRepository.UpdateAsync(existing, cancellationToken);
        _logger.LogInformation("Appointment {AppointmentId} updated", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _appointmentRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<IReadOnlyList<AppointmentDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var entities = await _appointmentRepository.SearchAsync(query, cancellationToken);
        return _mapper.Map<IReadOnlyList<AppointmentDto>>(entities);
    }

    public async Task<DashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await _doctorRepository.GetAllAsync(cancellationToken);
        var patients = await _patientRepository.GetAllAsync(cancellationToken);
        var staff = await _staffRepository.GetAllAsync(cancellationToken);
        var appointments = await _appointmentRepository.GetAllAsync(cancellationToken);

        return new DashboardDto
        {
            DoctorCount = doctors.Count,
            PatientCount = patients.Count,
            StaffCount = staff.Count,
            AppointmentCount = appointments.Count,
            Revenue = doctors.Sum(d => d.ChargesPerVisit)
        };
    }
}

using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public sealed class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(IAppointmentRepository repository, IMapper mapper, ILogger<AppointmentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AppointmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<AppointmentDto>>(entities);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<AppointmentDto>(entity);
    }

    public async Task<AppointmentDto> CreateAsync(AppointmentCreateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<Appointment>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Created appointment {AppointmentId}", created.Id);
            return _mapper.Map<AppointmentDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create appointment");
            throw;
        }
    }

    public async Task UpdateAsync(int id, AppointmentUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Appointment not found.");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken) => _repository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<AppointmentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        var entities = await _repository.SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IReadOnlyList<AppointmentDto>>(entities);
    }
}

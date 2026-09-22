using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public class PatientService : IPatientService
{
    private readonly IPatientRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<PatientService> _logger;

    public PatientService(IPatientRepository repository, IMapper mapper, ILogger<PatientService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PatientDto>>(entities);
    }

    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<PatientDto>(entity);
    }

    public async Task<PatientDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<Patient>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Patient {PatientName} created", created.Name);
            return _mapper.Map<PatientDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating patient");
            throw;
        }
    }

    public async Task UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Patient not found.");
        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing, cancellationToken);
        _logger.LogInformation("Patient {PatientId} updated", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Patient {PatientId} deleted", id);
    }

    public async Task<IReadOnlyList<PatientDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.SearchAsync(query, cancellationToken);
        return _mapper.Map<IReadOnlyList<PatientDto>>(entities);
    }
}

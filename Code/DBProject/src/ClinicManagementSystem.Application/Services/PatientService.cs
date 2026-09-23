using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public sealed class PatientService : IPatientService
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

    public async Task<IReadOnlyList<PatientDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<PatientDto>>(entities);
    }

    public async Task<PatientDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<PatientDto>(entity);
    }

    public async Task<PatientDto> CreateAsync(PatientCreateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<Patient>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Created patient {PatientId}", created.Id);
            return _mapper.Map<PatientDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create patient");
            throw;
        }
    }

    public async Task UpdateAsync(int id, PatientUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Patient not found.");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken) => _repository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<PatientDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        var entities = await _repository.SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IReadOnlyList<PatientDto>>(entities);
    }
}

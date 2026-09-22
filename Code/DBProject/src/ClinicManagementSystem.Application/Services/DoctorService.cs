using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DoctorService> _logger;

    public DoctorService(IDoctorRepository repository, IMapper mapper, ILogger<DoctorService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DoctorDto>>(entities);
    }

    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<DoctorDto>(entity);
    }

    public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<Doctor>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Doctor {DoctorName} created", created.Name);
            return _mapper.Map<DoctorDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating doctor");
            throw;
        }
    }

    public async Task UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Doctor not found.");
        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing, cancellationToken);
        _logger.LogInformation("Doctor {DoctorId} updated", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Doctor {DoctorId} deleted", id);
    }

    public async Task<IReadOnlyList<DoctorDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.SearchAsync(query, cancellationToken);
        return _mapper.Map<IReadOnlyList<DoctorDto>>(entities);
    }
}

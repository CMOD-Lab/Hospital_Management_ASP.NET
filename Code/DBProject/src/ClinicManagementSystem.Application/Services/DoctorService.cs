using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public sealed class DoctorService : IDoctorService
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

    public async Task<IReadOnlyList<DoctorDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DoctorDto>>(entities);
    }

    public async Task<DoctorDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<DoctorDto>(entity);
    }

    public async Task<DoctorDto> CreateAsync(DoctorCreateDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<Doctor>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Created doctor {DoctorId}", created.Id);
            return _mapper.Map<DoctorDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create doctor");
            throw;
        }
    }

    public async Task UpdateAsync(int id, DoctorUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Doctor not found.");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken) => _repository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<DoctorDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        var entities = await _repository.SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IReadOnlyList<DoctorDto>>(entities);
    }
}

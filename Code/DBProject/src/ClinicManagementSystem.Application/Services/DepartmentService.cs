using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public sealed class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository repository, IMapper mapper, ILogger<DepartmentService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentDto>>(entities);
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<DepartmentDto>(entity);
    }

    public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Department>(dto);
        var created = await _repository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created department {DepartmentId}", created.Id);
        return _mapper.Map<DepartmentDto>(created);
    }

    public async Task UpdateAsync(int id, DepartmentUpdateDto dto, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Department not found.");
        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken) => _repository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyList<DepartmentDto>> SearchAsync(string searchTerm, CancellationToken cancellationToken)
    {
        var entities = await _repository.SearchAsync(searchTerm, cancellationToken);
        return _mapper.Map<IReadOnlyList<DepartmentDto>>(entities);
    }
}

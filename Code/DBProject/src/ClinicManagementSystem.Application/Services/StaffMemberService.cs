using AutoMapper;
using ClinicManagementSystem.Application.DTOs;
using ClinicManagementSystem.Domain.Entities;
using ClinicManagementSystem.Domain.Interfaces.Repositories;
using ClinicManagementSystem.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagementSystem.Application.Services;

public class StaffMemberService : IStaffMemberService
{
    private readonly IStaffMemberRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogger<StaffMemberService> _logger;

    public StaffMemberService(IStaffMemberRepository repository, IMapper mapper, ILogger<StaffMemberService> logger)
    {
        _repository = repository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IReadOnlyList<StaffMemberDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<StaffMemberDto>>(entities);
    }

    public async Task<StaffMemberDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : _mapper.Map<StaffMemberDto>(entity);
    }

    public async Task<StaffMemberDto> CreateAsync(StaffMemberCreateDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var entity = _mapper.Map<StaffMember>(dto);
            var created = await _repository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Staff member {StaffName} created", created.Name);
            return _mapper.Map<StaffMemberDto>(created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            throw;
        }
    }

    public async Task UpdateAsync(int id, StaffMemberUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken) ?? throw new InvalidOperationException("Staff member not found.");
        _mapper.Map(dto, existing);
        await _repository.UpdateAsync(existing, cancellationToken);
        _logger.LogInformation("Staff member {StaffId} updated", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteAsync(id, cancellationToken);
        _logger.LogInformation("Staff member {StaffId} deleted", id);
    }

    public async Task<IReadOnlyList<StaffMemberDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.SearchAsync(query, cancellationToken);
        return _mapper.Map<IReadOnlyList<StaffMemberDto>>(entities);
    }
}

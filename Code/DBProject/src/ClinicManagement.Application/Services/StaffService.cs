using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for staff operations.
/// </summary>
public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<StaffService> _logger;

    public StaffService(IStaffRepository staffRepository, ILogger<StaffService> logger)
    {
        _staffRepository = staffRepository;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _staffRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID: {StaffId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<OtherStaff>> GetAllStaffAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all staff");
            return await _staffRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<IEnumerable<OtherStaff>> SearchStaffAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _staffRepository.SearchAsync(searchQuery, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query: {Query}", searchQuery);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<bool> AddStaffAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Adding new staff member: {StaffName}", staff.Name);
            await _staffRepository.AddAsync(staff, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff member: {StaffName}", staff.Name);
            return false;
        }
    }

    public async Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Deleting staff member with ID: {StaffId}", id);
            await _staffRepository.DeleteAsync(id, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member with ID: {StaffId}", id);
            return false;
        }
    }
}

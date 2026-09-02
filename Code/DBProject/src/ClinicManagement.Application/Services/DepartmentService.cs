using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Application.Services;

/// <summary>
/// Service implementation for department operations.
/// </summary>
public class DepartmentService : IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;
    private readonly ILogger<DepartmentService> _logger;

    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
    {
        _departmentRepository = departmentRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Retrieving all departments");
            return await _departmentRepository.GetAllAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            return Enumerable.Empty<Department>();
        }
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _departmentRepository.GetByIdAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID: {DeptId}", id);
            return null;
        }
    }

    public async Task<Department?> GetDepartmentByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _departmentRepository.GetByNameAsync(name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with name: {DeptName}", name);
            return null;
        }
    }
}

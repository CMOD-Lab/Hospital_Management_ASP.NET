using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>
/// Service interface for doctor operations.
/// </summary>
public interface IDoctorService
{
    Task<Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> SearchDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetDoctorsByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(Doctor doctor, string email, string password, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

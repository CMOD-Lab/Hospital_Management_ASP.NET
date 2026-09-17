using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Services;

/// <summary>Service interface for admin operations.</summary>
public interface IAdminService
{
    Task<(int TotalDoctors, int TotalPatients, decimal TotalIncome)> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Appointment>> GetAllAppointmentsAsync(CancellationToken cancellationToken = default);
    Task<bool> AddDoctorAsync(Doctor doctor, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<bool> AddStaffAsync(Staff staff, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int staffId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Staff>> GetAllStaffAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetAllDoctorsAsync(string searchQuery = "", CancellationToken cancellationToken = default);
    Task<bool> CheckDoctorEmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

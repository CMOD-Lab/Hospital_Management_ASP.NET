using CareTrack.Domain.Entities;

namespace CareTrack.Domain.Interfaces.Services;

/// <summary>
/// Service interface for admin operations.
/// </summary>
public interface IAdminService
{
    Task<AdminDashboardData> GetDashboardDataAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Doctor>> GetDoctorsAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<Patient>> GetPatientsAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> GetStaffAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> AddDoctorAsync(Doctor doctor, string email, string password, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> AddStaffAsync(OtherStaff staff, CancellationToken cancellationToken = default);
    Task<bool> DeleteDoctorAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> DeleteStaffAsync(int id, CancellationToken cancellationToken = default);
    Task<Doctor?> GetDoctorByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OtherStaff?> GetStaffByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Patient?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Department>> GetDepartmentsAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Data transfer object for admin dashboard statistics.
/// </summary>
public class AdminDashboardData
{
    public int TotalDoctors { get; set; }
    public int TotalPatients { get; set; }
    public double TotalIncome { get; set; }
    public IEnumerable<DepartmentStat> DepartmentStats { get; set; } = new List<DepartmentStat>();
    public IEnumerable<AppointmentStat> AppointmentStats { get; set; } = new List<AppointmentStat>();
}

public class DepartmentStat
{
    public string DeptName { get; set; } = string.Empty;
    public int DoctorCount { get; set; }
}

public class AppointmentStat
{
    public int AppointId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public DateTime? Date { get; set; }
    public string Status { get; set; } = string.Empty;
}

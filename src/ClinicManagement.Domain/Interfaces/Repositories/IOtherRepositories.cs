using ClinicManagement.Domain.Entities;

namespace ClinicManagement.Domain.Interfaces.Repositories;

/// <summary>Staff repository interface</summary>
public interface IStaffRepository
{
    Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default);
    Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default);
    Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default);
}

/// <summary>Department repository interface</summary>
public interface IDepartmentRepository
{
    Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}

/// <summary>Bill repository interface</summary>
public interface IBillRepository
{
    Task<IEnumerable<Bill>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Bill>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default);
    Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default);
    Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalIncomeAsync(CancellationToken cancellationToken = default);
}

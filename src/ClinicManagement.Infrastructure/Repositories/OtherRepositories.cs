using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>Staff repository implementation</summary>
public class StaffRepository : IStaffRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(ClinicDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id && s.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID: {Id}", id);
            return null;
        }
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .Where(s => s.IsActive)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all staff");
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff.AsNoTracking()
                .Where(s => s.IsActive && s.Name.Contains(searchQuery))
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching staff with query: {Query}", searchQuery);
            return Enumerable.Empty<OtherStaff>();
        }
    }

    public async Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.OtherStaff.Add(staff);
            await _context.SaveChangesAsync(cancellationToken);
            return staff;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding staff: {Name}", staff.Name);
            throw;
        }
    }

    public async Task<bool> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var staff = await _context.OtherStaff.FindAsync(new object[] { id }, cancellationToken);
            if (staff == null) return false;
            staff.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating staff with ID: {Id}", id);
            return false;
        }
    }
}

/// <summary>Department repository implementation</summary>
public class DepartmentRepository : IDepartmentRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DepartmentRepository> _logger;

    public DepartmentRepository(ClinicDbContext context, ILogger<DepartmentRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Department>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .Include(d => d.Doctors)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all departments");
            return Enumerable.Empty<Department>();
        }
    }

    public async Task<Department?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptNo == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with ID: {Id}", id);
            return null;
        }
    }

    public async Task<Department?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Departments.AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptName == name, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving department with name: {Name}", name);
            return null;
        }
    }
}

/// <summary>Bill repository implementation</summary>
public class BillRepository : IBillRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<BillRepository> _logger;

    public BillRepository(ClinicDbContext context, ILogger<BillRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Bill>> GetByPatientAsync(int patientId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills.AsNoTracking()
                .Include(b => b.Patient)
                .Include(b => b.Doctor)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for patient: {PatientId}", patientId);
            return Enumerable.Empty<Bill>();
        }
    }

    public async Task<IEnumerable<Bill>> GetByDoctorAsync(int doctorId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills.AsNoTracking()
                .Include(b => b.Patient)
                .Where(b => b.DoctorId == doctorId)
                .OrderByDescending(b => b.BillDate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving bills for doctor: {DoctorId}", doctorId);
            return Enumerable.Empty<Bill>();
        }
    }

    public async Task<Bill> AddAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bills.Add(bill);
            await _context.SaveChangesAsync(cancellationToken);
            return bill;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bill");
            throw;
        }
    }

    public async Task UpdateAsync(Bill bill, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Bills.Update(bill);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating bill with ID: {Id}", bill.BillId);
            throw;
        }
    }

    public async Task<decimal> GetTotalIncomeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Bills
                .Where(b => b.IsPaid)
                .SumAsync(b => b.Amount, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating total income");
            return 0;
        }
    }
}

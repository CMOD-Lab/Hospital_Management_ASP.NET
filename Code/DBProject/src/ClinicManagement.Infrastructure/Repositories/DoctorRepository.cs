using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the doctor repository.
/// </summary>
public class DoctorRepository : IDoctorRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<DoctorRepository> _logger;

    public DoctorRepository(ClinicDbContext context, ILogger<DoctorRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Doctor?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.DoctorId == id, cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status == 1)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status == 1 && d.Name.Contains(searchQuery))
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Doctor>> GetByDepartmentAsync(string departmentName, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .Include(d => d.Department)
            .Where(d => d.Status == 1 && d.Department != null && d.Department.DeptName == departmentName)
            .OrderBy(d => d.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Doctor> AddAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync(cancellationToken);
        return doctor;
    }

    public async Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken = default)
    {
        _context.Doctors.Update(doctor);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        // Soft delete - set status to 0
        var doctor = await _context.Doctors.FindAsync(new object[] { id }, cancellationToken);
        if (doctor != null)
        {
            doctor.Status = 0;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Doctors
            .AsNoTracking()
            .AnyAsync(d => d.DoctorId == id, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginEntries
            .AsNoTracking()
            .AnyAsync(l => l.Email == email, cancellationToken);
    }
}

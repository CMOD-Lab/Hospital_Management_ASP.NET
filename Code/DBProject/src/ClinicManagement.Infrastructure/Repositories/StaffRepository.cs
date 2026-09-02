using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the staff repository.
/// </summary>
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
        return await _context.OtherStaff
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.StaffId == id, cancellationToken);
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OtherStaff>> SearchAsync(string searchQuery, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff
            .AsNoTracking()
            .Where(s => s.Name.Contains(searchQuery))
            .OrderBy(s => s.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<OtherStaff> AddAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        _context.OtherStaff.Add(staff);
        await _context.SaveChangesAsync(cancellationToken);
        return staff;
    }

    public async Task UpdateAsync(OtherStaff staff, CancellationToken cancellationToken = default)
    {
        _context.OtherStaff.Update(staff);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var staff = await _context.OtherStaff.FindAsync(new object[] { id }, cancellationToken);
        if (staff != null)
        {
            _context.OtherStaff.Remove(staff);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.OtherStaff
            .AsNoTracking()
            .AnyAsync(s => s.StaffId == id, cancellationToken);
    }
}

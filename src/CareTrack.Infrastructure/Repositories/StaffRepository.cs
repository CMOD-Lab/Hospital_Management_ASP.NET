using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareTrack.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the staff repository.
/// </summary>
public class StaffRepository : IStaffRepository
{
    private readonly CareTrackDbContext _context;
    private readonly ILogger<StaffRepository> _logger;

    public StaffRepository(CareTrackDbContext context, ILogger<StaffRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<OtherStaff?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StaffId == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving staff with ID: {StaffId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<OtherStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.OtherStaff
                .AsNoTracking()
                .OrderBy(s => s.Name)
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
            return await _context.OtherStaff
                .AsNoTracking()
                .Where(s => s.Name.Contains(searchQuery))
                .OrderBy(s => s.Name)
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
}

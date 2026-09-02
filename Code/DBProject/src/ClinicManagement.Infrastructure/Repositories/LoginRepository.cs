using ClinicManagement.Domain.Entities;
using ClinicManagement.Domain.Interfaces.Repositories;
using ClinicManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicManagement.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the login repository.
/// </summary>
public class LoginRepository : ILoginRepository
{
    private readonly ClinicDbContext _context;
    private readonly ILogger<LoginRepository> _logger;

    public LoginRepository(ClinicDbContext context, ILogger<LoginRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LoginEntry?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email && l.Password == password, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", email);
            return null;
        }
    }

    public async Task<LoginEntry?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginEntries
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == email, cancellationToken);
    }

    public async Task<LoginEntry> AddAsync(LoginEntry loginEntry, CancellationToken cancellationToken = default)
    {
        _context.LoginEntries.Add(loginEntry);
        await _context.SaveChangesAsync(cancellationToken);
        return loginEntry;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginEntries
            .AsNoTracking()
            .AnyAsync(l => l.Email == email, cancellationToken);
    }
}

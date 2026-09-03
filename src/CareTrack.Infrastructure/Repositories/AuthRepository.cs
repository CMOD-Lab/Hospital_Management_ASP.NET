using CareTrack.Domain.Entities;
using CareTrack.Domain.Interfaces.Repositories;
using CareTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CareTrack.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the auth repository.
/// </summary>
public class AuthRepository : IAuthRepository
{
    private readonly CareTrackDbContext _context;
    private readonly ILogger<AuthRepository> _logger;

    public AuthRepository(CareTrackDbContext context, ILogger<AuthRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<LoginTable?> ValidateLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.LoginTable
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Email == email && l.Password == password, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating login for email: {Email}", email);
            return null;
        }
    }

    public async Task<LoginTable> CreateLoginAsync(LoginTable login, CancellationToken cancellationToken = default)
    {
        _context.LoginTable.Add(login);
        await _context.SaveChangesAsync(cancellationToken);
        return login;
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginTable
            .AsNoTracking()
            .AnyAsync(l => l.Email == email, cancellationToken);
    }

    public async Task<LoginTable?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.LoginTable
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Email == email, cancellationToken);
    }
}

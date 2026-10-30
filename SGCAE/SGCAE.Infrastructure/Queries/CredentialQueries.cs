using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Credentials;
using SGCAE.Domain.Credentials;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

public sealed class CredentialQueries(AppDbContext db) : ICredentialQueries
{
    public async Task<CredentialDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var credential = await db.Credentials.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return credential is null ? null : Map(credential);
    }

    public async Task<IReadOnlyList<CredentialDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct = default)
    {
        var items = await db.Credentials.AsNoTracking()
            .Where(c => c.EmployeeId == employeeId)
            .OrderByDescending(c => c.IssuedAt)
            .ToListAsync(ct);
        return items.Select(Map).ToList();
    }

    private static CredentialDto Map(Credential c)
        => new(c.Id, c.Code, c.EmployeeId, c.Status.ToString(), c.IssuedAt, c.StatusChangedAt);
}

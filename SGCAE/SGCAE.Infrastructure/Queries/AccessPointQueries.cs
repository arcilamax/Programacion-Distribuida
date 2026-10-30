using Microsoft.EntityFrameworkCore;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.Abstractions;
using SGCAE.Domain.AccessPoints;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

public sealed class AccessPointQueries(AppDbContext db) : IAccessPointQueries
{
    public async Task<AccessPointDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var point = await db.AccessPoints.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);
        return point is null ? null : Map(point);
    }

    public async Task<IReadOnlyList<AccessPointDto>> GetAllAsync(bool onlyEnabled, CancellationToken ct = default)
    {
        var query = db.AccessPoints.AsNoTracking();
        if (onlyEnabled) query = query.Where(a => a.IsEnabled);

        var items = await query.OrderBy(a => a.Name).ToListAsync(ct);
        return items.Select(Map).ToList();
    }

    private static AccessPointDto Map(AccessPoint a) => new(a.Id, a.Name, a.Description, a.IsEnabled);
}

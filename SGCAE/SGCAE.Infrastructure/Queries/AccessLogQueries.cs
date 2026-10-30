using Microsoft.EntityFrameworkCore;
using SGCAE.Application.AccessLogs;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

public sealed class AccessLogQueries(AppDbContext db) : IAccessLogQueries
{
    public async Task<PagedResult<AccessLogDto>> SearchAsync(AccessLogFilter f, CancellationToken ct = default)
    {
        // LEFT JOIN con empleados y puntos de acceso (pueden ser nulos en accesos rechazados).
        var query =
            from log in db.AccessLogs.AsNoTracking()
            join emp0 in db.Employees.AsNoTracking() on log.EmployeeId equals (Guid?)emp0.Id into empJoin
            from emp in empJoin.DefaultIfEmpty()
            join pt0 in db.AccessPoints.AsNoTracking() on log.AccessPointId equals (Guid?)pt0.Id into ptJoin
            from pt in ptJoin.DefaultIfEmpty()
            select new
            {
                log.Id,
                log.EmployeeId,
                EmployeeFirstName = emp.FirstName,
                EmployeeLastName = emp.LastName,
                log.CredentialCode,
                log.AccessPointId,
                AccessPointName = pt.Name,
                log.Movement,
                log.Result,
                log.Reason,
                log.OccurredAt
            };

        if (f.EmployeeId.HasValue)
        {
            var employeeId = f.EmployeeId.Value;
            query = query.Where(x => x.EmployeeId == employeeId);
        }
        if (f.AccessPointId.HasValue)
        {
            var pointId = f.AccessPointId.Value;
            query = query.Where(x => x.AccessPointId == pointId);
        }
        if (f.Result.HasValue)
        {
            var result = f.Result.Value;
            query = query.Where(x => x.Result == result);
        }
        if (f.From.HasValue)
        {
            var fromDate = f.From.Value;
            query = query.Where(x => x.OccurredAt >= fromDate);
        }
        if (f.To.HasValue)
        {
            var toDate = f.To.Value;
            query = query.Where(x => x.OccurredAt <= toDate);
        }

        var total = await query.CountAsync(ct);

        var rows = await query
            .OrderByDescending(x => x.OccurredAt)
            .Skip((f.Page - 1) * f.PageSize)
            .Take(f.PageSize)
            .ToListAsync(ct);

        var items = rows.Select(x => new AccessLogDto(
            x.Id,
            x.EmployeeId,
            x.EmployeeFirstName is null ? null : $"{x.EmployeeFirstName} {x.EmployeeLastName}",
            x.CredentialCode,
            x.AccessPointId,
            x.AccessPointName,
            x.Movement.ToString(),
            x.Result.ToString(),
            x.Reason?.ToString(),
            x.OccurredAt)).ToList();

        return new PagedResult<AccessLogDto>(items, f.Page, f.PageSize, total);
    }
}

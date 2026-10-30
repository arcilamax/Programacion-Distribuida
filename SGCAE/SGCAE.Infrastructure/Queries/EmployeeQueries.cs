using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Application.Employees;
using SGCAE.Domain.Employees;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Queries;

public sealed class EmployeeQueries(AppDbContext db) : IEmployeeQueries
{
    public async Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var employee = await db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        return employee is null ? null : Map(employee);
    }

    public async Task<PagedResult<EmployeeDto>> SearchAsync(EmployeeStatus? status, int page, int pageSize,
        CancellationToken ct = default)
    {
        var query = db.Employees.AsNoTracking();
        if (status.HasValue)
        {
            var value = status.Value;
            query = query.Where(e => e.Status == value);
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(e => e.LastName).ThenBy(e => e.FirstName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<EmployeeDto>(items.Select(Map).ToList(), page, pageSize, total);
    }

    private static EmployeeDto Map(Employee e) => new(e.Id, e.Identification, e.FirstName, e.LastName,
        e.Email.Value, e.Position, e.Area, e.Status.ToString(), e.CreatedAt);
}

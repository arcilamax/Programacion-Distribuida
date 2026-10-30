using Microsoft.EntityFrameworkCore;
using SGCAE.Application.Abstractions;
using SGCAE.Domain.AccessLogs;
using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;
using SGCAE.Infrastructure.Persistence;

namespace SGCAE.Infrastructure.Repositories;

public sealed class EmployeeRepository(AppDbContext db) : IEmployeeRepository
{
    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<bool> ExistsByIdentificationAsync(string identification, CancellationToken ct = default)
        => db.Employees.AnyAsync(e => e.Identification == identification, ct);

    public Task<bool> ExistsByEmailAsync(Email email, Guid? excludingId, CancellationToken ct = default)
        => db.Employees.AnyAsync(e => e.Email == email && (excludingId == null || e.Id != excludingId), ct);

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
        => await db.Employees.AddAsync(employee, ct);
}

public sealed class CredentialRepository(AppDbContext db) : ICredentialRepository
{
    public Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Credentials.FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Credential?> GetByCodeAsync(string code, CancellationToken ct = default)
        => db.Credentials.FirstOrDefaultAsync(c => c.Code == code, ct);

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default)
        => db.Credentials.AnyAsync(c => c.Code == code, ct);

    public async Task AddAsync(Credential credential, CancellationToken ct = default)
        => await db.Credentials.AddAsync(credential, ct);
}

public sealed class AccessPointRepository(AppDbContext db) : IAccessPointRepository
{
    public Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.AccessPoints.FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        => db.AccessPoints.AnyAsync(a => a.Name == name, ct);

    public async Task AddAsync(AccessPoint accessPoint, CancellationToken ct = default)
        => await db.AccessPoints.AddAsync(accessPoint, ct);
}

public sealed class AccessLogRepository(AppDbContext db) : IAccessLogRepository
{
    public async Task AddAsync(AccessLog log, CancellationToken ct = default)
        => await db.AccessLogs.AddAsync(log, ct);
}

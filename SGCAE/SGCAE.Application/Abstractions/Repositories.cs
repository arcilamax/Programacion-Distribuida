using SGCAE.Domain.AccessLogs;
using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;

namespace SGCAE.Application.Abstractions;

// Lado ESCRITURA (Commands): trabajan con entidades del dominio.

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByIdentificationAsync(string identification, CancellationToken ct = default);
    Task<bool> ExistsByEmailAsync(Email email, Guid? excludingId, CancellationToken ct = default);
    Task AddAsync(Employee employee, CancellationToken ct = default);
}

public interface ICredentialRepository
{
    Task<Credential?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Credential?> GetByCodeAsync(string code, CancellationToken ct = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task AddAsync(Credential credential, CancellationToken ct = default);
}

public interface IAccessPointRepository
{
    Task<AccessPoint?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
    Task AddAsync(AccessPoint accessPoint, CancellationToken ct = default);
}

public interface IAccessLogRepository
{
    Task AddAsync(AccessLog log, CancellationToken ct = default);
}

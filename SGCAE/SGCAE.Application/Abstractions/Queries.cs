using SGCAE.Application.AccessLogs;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.Common;
using SGCAE.Application.Credentials;
using SGCAE.Application.Employees;
using SGCAE.Domain.AccessLogs;
using SGCAE.Domain.Employees;

namespace SGCAE.Application.Abstractions;

// Lado LECTURA (Queries): devuelven DTOs directamente, sin pasar por las entidades.

public interface IEmployeeQueries
{
    Task<EmployeeDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<EmployeeDto>> SearchAsync(EmployeeStatus? status, int page, int pageSize, CancellationToken ct = default);
}

public interface ICredentialQueries
{
    Task<CredentialDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CredentialDto>> GetByEmployeeAsync(Guid employeeId, CancellationToken ct = default);
}

public interface IAccessPointQueries
{
    Task<AccessPointDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<AccessPointDto>> GetAllAsync(bool onlyEnabled, CancellationToken ct = default);
}

public sealed record AccessLogFilter(Guid? EmployeeId, DateTime? From, DateTime? To,
    Guid? AccessPointId, AccessResult? Result, int Page, int PageSize);

public interface IAccessLogQueries
{
    Task<PagedResult<AccessLogDto>> SearchAsync(AccessLogFilter filter, CancellationToken ct = default);
}

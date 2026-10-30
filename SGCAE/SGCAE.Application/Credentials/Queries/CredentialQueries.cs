using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;

namespace SGCAE.Application.Credentials.Queries;

public sealed record GetCredentialByIdQuery(Guid Id) : IRequest<CredentialDto>;

public sealed class GetCredentialByIdHandler(ICredentialQueries queries)
    : IRequestHandler<GetCredentialByIdQuery, CredentialDto>
{
    public async Task<CredentialDto> Handle(GetCredentialByIdQuery request, CancellationToken ct)
        => await queries.GetByIdAsync(request.Id, ct)
           ?? throw new NotFoundException("Credencial", request.Id);
}

public sealed record GetCredentialsByEmployeeQuery(Guid EmployeeId) : IRequest<IReadOnlyList<CredentialDto>>;

public sealed class GetCredentialsByEmployeeHandler(ICredentialQueries queries)
    : IRequestHandler<GetCredentialsByEmployeeQuery, IReadOnlyList<CredentialDto>>
{
    public Task<IReadOnlyList<CredentialDto>> Handle(GetCredentialsByEmployeeQuery request, CancellationToken ct)
        => queries.GetByEmployeeAsync(request.EmployeeId, ct);
}

using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;

namespace SGCAE.Application.AccessPoints.Queries;

public sealed record GetAccessPointByIdQuery(Guid Id) : IRequest<AccessPointDto>;

public sealed class GetAccessPointByIdHandler(IAccessPointQueries queries)
    : IRequestHandler<GetAccessPointByIdQuery, AccessPointDto>
{
    public async Task<AccessPointDto> Handle(GetAccessPointByIdQuery request, CancellationToken ct)
        => await queries.GetByIdAsync(request.Id, ct)
           ?? throw new NotFoundException("Punto de acceso", request.Id);
}

public sealed record GetAccessPointsQuery(bool OnlyEnabled = false) : IRequest<IReadOnlyList<AccessPointDto>>;

public sealed class GetAccessPointsHandler(IAccessPointQueries queries)
    : IRequestHandler<GetAccessPointsQuery, IReadOnlyList<AccessPointDto>>
{
    public Task<IReadOnlyList<AccessPointDto>> Handle(GetAccessPointsQuery request, CancellationToken ct)
        => queries.GetAllAsync(request.OnlyEnabled, ct);
}

using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;

namespace SGCAE.Application.Credentials.Commands;

public sealed record BlockCredentialCommand(Guid Id) : IRequest;
public sealed record ReactivateCredentialCommand(Guid Id) : IRequest;
public sealed record RevokeCredentialCommand(Guid Id) : IRequest;

public sealed class BlockCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow)
    : IRequestHandler<BlockCredentialCommand>
{
    public async Task Handle(BlockCredentialCommand request, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Credencial", request.Id);
        credential.Block();
        await uow.SaveChangesAsync(ct);
    }
}

public sealed class ReactivateCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow)
    : IRequestHandler<ReactivateCredentialCommand>
{
    public async Task Handle(ReactivateCredentialCommand request, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Credencial", request.Id);
        credential.Reactivate();
        await uow.SaveChangesAsync(ct);
    }
}

public sealed class RevokeCredentialHandler(ICredentialRepository credentials, IUnitOfWork uow)
    : IRequestHandler<RevokeCredentialCommand>
{
    public async Task Handle(RevokeCredentialCommand request, CancellationToken ct)
    {
        var credential = await credentials.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Credencial", request.Id);
        credential.Revoke();
        await uow.SaveChangesAsync(ct);
    }
}

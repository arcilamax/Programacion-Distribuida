using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;
using SGCAE.Domain.AccessPoints;

namespace SGCAE.Application.AccessPoints.Commands;

public sealed record CreateAccessPointCommand(string Name, string? Description) : IRequest<Guid>;

public sealed class CreateAccessPointValidator : AbstractValidator<CreateAccessPointCommand>
{
    public CreateAccessPointValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(300);
    }
}

public sealed class CreateAccessPointHandler(IAccessPointRepository points, IUnitOfWork uow)
    : IRequestHandler<CreateAccessPointCommand, Guid>
{
    public async Task<Guid> Handle(CreateAccessPointCommand request, CancellationToken ct)
    {
        var point = AccessPoint.Create(request.Name, request.Description);

        if (await points.ExistsByNameAsync(point.Name, ct))
            throw new ConflictException($"Ya existe un punto de acceso llamado '{point.Name}'.");

        await points.AddAsync(point, ct);
        await uow.SaveChangesAsync(ct);
        return point.Id;
    }
}

public sealed record EnableAccessPointCommand(Guid Id) : IRequest;
public sealed record DisableAccessPointCommand(Guid Id) : IRequest;

public sealed class EnableAccessPointHandler(IAccessPointRepository points, IUnitOfWork uow)
    : IRequestHandler<EnableAccessPointCommand>
{
    public async Task Handle(EnableAccessPointCommand request, CancellationToken ct)
    {
        var point = await points.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Punto de acceso", request.Id);
        point.Enable();
        await uow.SaveChangesAsync(ct);
    }
}

public sealed class DisableAccessPointHandler(IAccessPointRepository points, IUnitOfWork uow)
    : IRequestHandler<DisableAccessPointCommand>
{
    public async Task Handle(DisableAccessPointCommand request, CancellationToken ct)
    {
        var point = await points.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Punto de acceso", request.Id);
        point.Disable();
        await uow.SaveChangesAsync(ct);
    }
}

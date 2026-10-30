using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;
using SGCAE.Domain.Credentials;

namespace SGCAE.Application.Credentials.Commands;

public sealed record AssignCredentialCommand(Guid EmployeeId, string Code) : IRequest<Guid>;

public sealed class AssignCredentialValidator : AbstractValidator<AssignCredentialCommand>
{
    public AssignCredentialValidator()
    {
        RuleFor(x => x.EmployeeId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
    }
}

public sealed class AssignCredentialHandler(IEmployeeRepository employees, ICredentialRepository credentials,
    IUnitOfWork uow) : IRequestHandler<AssignCredentialCommand, Guid>
{
    public async Task<Guid> Handle(AssignCredentialCommand request, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(request.EmployeeId, ct)
            ?? throw new NotFoundException("Empleado", request.EmployeeId);

        var credential = Credential.Issue(employee, request.Code);

        if (await credentials.ExistsByCodeAsync(credential.Code, ct))
            throw new ConflictException($"Ya existe una credencial con el código '{credential.Code}'.");

        await credentials.AddAsync(credential, ct);
        await uow.SaveChangesAsync(ct);
        return credential.Id;
    }
}

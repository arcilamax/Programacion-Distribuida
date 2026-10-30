using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Domain.AccessLogs;

namespace SGCAE.Application.AccessLogs.Commands;

public sealed record RegisterAccessCommand(string CredentialCode, Guid AccessPointId, MovementType Movement)
    : IRequest<AccessLogDto>;

public sealed class RegisterAccessValidator : AbstractValidator<RegisterAccessCommand>
{
    public RegisterAccessValidator()
    {
        RuleFor(x => x.CredentialCode).NotEmpty().MaximumLength(50);
        RuleFor(x => x.AccessPointId).NotEmpty();
        RuleFor(x => x.Movement).IsInEnum();
    }
}

public sealed class RegisterAccessHandler(
    ICredentialRepository credentials,
    IEmployeeRepository employees,
    IAccessPointRepository accessPoints,
    IAccessLogRepository logs,
    IUnitOfWork uow) : IRequestHandler<RegisterAccessCommand, AccessLogDto>
{
    public async Task<AccessLogDto> Handle(RegisterAccessCommand request, CancellationToken ct)
    {
        var code = request.CredentialCode.Trim();

        var credential = await credentials.GetByCodeAsync(code, ct);
        var employee = credential is null ? null : await employees.GetByIdAsync(credential.EmployeeId, ct);
        var point = await accessPoints.GetByIdAsync(request.AccessPointId, ct);

        // El dominio decide si se autoriza o se rechaza (y por qué).
        var log = AccessLog.Register(code, request.Movement, credential, employee, point);

        // Siempre se guarda: autorizado o rechazado.
        await logs.AddAsync(log, ct);
        await uow.SaveChangesAsync(ct);

        return new AccessLogDto(
            log.Id,
            log.EmployeeId,
            employee is null ? null : $"{employee.FirstName} {employee.LastName}",
            log.CredentialCode,
            log.AccessPointId,
            point?.Name,
            log.Movement.ToString(),
            log.Result.ToString(),
            log.Reason?.ToString(),
            log.OccurredAt);
    }
}

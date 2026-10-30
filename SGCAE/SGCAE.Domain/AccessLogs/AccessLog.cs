using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.Common;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;

namespace SGCAE.Domain.AccessLogs;

/// <summary>Un registro de intento de acceso. Se guarda siempre, sea autorizado o rechazado.</summary>
public sealed class AccessLog : Entity
{
    public Guid? EmployeeId { get; private set; }
    public string CredentialCode { get; private set; } = default!;
    public Guid? AccessPointId { get; private set; }
    public MovementType Movement { get; private set; }
    public AccessResult Result { get; private set; }
    public RejectionReason? Reason { get; private set; }
    public DateTime OccurredAt { get; private set; }

    private AccessLog() { } // requerido por EF Core

    /// <summary>
    /// Aplica TODAS las reglas de acceso y construye el registro resultante.
    /// Las entidades llegan nulas cuando no existen (credencial o punto desconocido).
    /// </summary>
    public static AccessLog Register(string credentialCode, MovementType movement,
        Credential? credential, Employee? employee, AccessPoint? accessPoint)
    {
        var reason = Evaluate(credential, employee, accessPoint);

        return new AccessLog
        {
            Id = Guid.NewGuid(),
            EmployeeId = employee?.Id,
            CredentialCode = Guard.Required(credentialCode, "El código de la credencial", 50),
            AccessPointId = accessPoint?.Id,
            Movement = movement,
            Result = reason is null ? AccessResult.Authorized : AccessResult.Rejected,
            Reason = reason,
            OccurredAt = DateTime.UtcNow
        };
    }

    private static RejectionReason? Evaluate(Credential? credential, Employee? employee, AccessPoint? accessPoint)
    {
        if (accessPoint is null) return RejectionReason.AccessPointNotFound;
        if (credential is null) return RejectionReason.CredentialNotFound;
        if (credential.Status == CredentialStatus.Revoked) return RejectionReason.CredentialRevoked;
        if (credential.Status == CredentialStatus.Blocked) return RejectionReason.CredentialBlocked;
        if (employee is null || !employee.IsActive) return RejectionReason.EmployeeInactive;
        if (!accessPoint.IsEnabled) return RejectionReason.AccessPointDisabled;
        return null;
    }
}

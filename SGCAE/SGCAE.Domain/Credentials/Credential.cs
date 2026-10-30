using SGCAE.Domain.Common;
using SGCAE.Domain.Employees;

namespace SGCAE.Domain.Credentials;

public sealed class Credential : Entity
{
    public string Code { get; private set; } = default!;
    public Guid EmployeeId { get; private set; }
    public CredentialStatus Status { get; private set; }
    public DateTime IssuedAt { get; private set; }
    public DateTime? StatusChangedAt { get; private set; }

    /// <summary>Solo una credencial activa puede utilizarse.</summary>
    public bool CanBeUsed => Status == CredentialStatus.Active;

    private Credential() { } // requerido por EF Core

    public static Credential Issue(Employee employee, string code)
    {
        if (!employee.IsActive)
            throw new DomainException("No se puede asignar una credencial a un empleado inactivo.");

        return new Credential
        {
            Id = Guid.NewGuid(),
            Code = Guard.Required(code, "El código de la credencial", 50),
            EmployeeId = employee.Id,
            Status = CredentialStatus.Active,
            IssuedAt = DateTime.UtcNow
        };
    }

    public void Block()
    {
        if (Status == CredentialStatus.Revoked)
            throw new DomainException("Una credencial revocada no puede bloquearse.");
        if (Status == CredentialStatus.Blocked)
            throw new DomainException("La credencial ya está bloqueada.");
        ChangeStatus(CredentialStatus.Blocked);
    }

    public void Reactivate()
    {
        if (Status == CredentialStatus.Revoked)
            throw new DomainException("Una credencial revocada no puede reactivarse.");
        if (Status == CredentialStatus.Active)
            throw new DomainException("La credencial ya está activa.");
        ChangeStatus(CredentialStatus.Active);
    }

    public void Revoke()
    {
        if (Status == CredentialStatus.Revoked)
            throw new DomainException("La credencial ya está revocada.");
        ChangeStatus(CredentialStatus.Revoked);
    }

    private void ChangeStatus(CredentialStatus status)
    {
        Status = status;
        StatusChangedAt = DateTime.UtcNow;
    }
}

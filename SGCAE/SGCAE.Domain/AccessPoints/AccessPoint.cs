using SGCAE.Domain.Common;

namespace SGCAE.Domain.AccessPoints;

public sealed class AccessPoint : Entity
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public bool IsEnabled { get; private set; }

    private AccessPoint() { } // requerido por EF Core

    public static AccessPoint Create(string name, string? description)
    {
        return new AccessPoint
        {
            Id = Guid.NewGuid(),
            Name = Guard.Required(name, "El nombre del punto de acceso", 100),
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            IsEnabled = true
        };
    }

    public void Enable()
    {
        if (IsEnabled) throw new DomainException("El punto de acceso ya está habilitado.");
        IsEnabled = true;
    }

    public void Disable()
    {
        if (!IsEnabled) throw new DomainException("El punto de acceso ya está deshabilitado.");
        IsEnabled = false;
    }
}

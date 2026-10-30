namespace SGCAE.Domain.Common;

/// <summary>Clase base de todas las entidades: se identifican por su Id, no por sus datos.</summary>
public abstract class Entity
{
    public Guid Id { get; protected set; }
}

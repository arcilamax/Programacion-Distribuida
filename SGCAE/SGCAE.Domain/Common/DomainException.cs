namespace SGCAE.Domain.Common;

/// <summary>Se lanza cuando se intenta violar una regla del negocio (invariante).</summary>
public sealed class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

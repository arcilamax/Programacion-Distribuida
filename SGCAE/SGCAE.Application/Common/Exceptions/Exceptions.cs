namespace SGCAE.Application.Common.Exceptions;

/// <summary>El recurso solicitado no existe (HTTP 404).</summary>
public sealed class NotFoundException : Exception
{
    public NotFoundException(string entity, object key)
        : base($"{entity} con identificador '{key}' no existe.") { }
}

/// <summary>El recurso ya existe o choca con otro (HTTP 409).</summary>
public sealed class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

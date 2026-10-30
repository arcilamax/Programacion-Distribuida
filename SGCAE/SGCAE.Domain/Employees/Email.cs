using System.Text.RegularExpressions;
using SGCAE.Domain.Common;

namespace SGCAE.Domain.Employees;

/// <summary>Objeto de valor: un correo siempre es válido y normalizado, o no existe.</summary>
public sealed record Email
{
    public string Value { get; }

    private Email(string value) => Value = value;

    public static Email Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("El correo electrónico es obligatorio.");

        var normalized = value.Trim().ToLowerInvariant();

        if (normalized.Length > 200 || !Regex.IsMatch(normalized, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new DomainException("El correo electrónico no tiene un formato válido.");

        return new Email(normalized);
    }

    public override string ToString() => Value;
}

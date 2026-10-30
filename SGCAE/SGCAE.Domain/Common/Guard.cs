namespace SGCAE.Domain.Common;

internal static class Guard
{
    /// <summary>Valida que un texto no esté vacío ni exceda el largo máximo, y lo devuelve sin espacios sobrantes.</summary>
    public static string Required(string? value, string field, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{field} es obligatorio.");

        var trimmed = value.Trim();
        if (trimmed.Length > maxLength)
            throw new DomainException($"{field} no puede superar {maxLength} caracteres.");

        return trimmed;
    }
}

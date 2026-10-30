namespace SGCAE.Application.Credentials;

public sealed record CredentialDto(Guid Id, string Code, Guid EmployeeId, string Status,
    DateTime IssuedAt, DateTime? StatusChangedAt);

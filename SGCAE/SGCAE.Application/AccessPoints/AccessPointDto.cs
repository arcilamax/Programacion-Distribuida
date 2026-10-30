namespace SGCAE.Application.AccessPoints;

public sealed record AccessPointDto(Guid Id, string Name, string? Description, bool IsEnabled);

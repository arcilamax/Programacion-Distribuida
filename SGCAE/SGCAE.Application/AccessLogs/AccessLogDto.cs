namespace SGCAE.Application.AccessLogs;

public sealed record AccessLogDto(Guid Id, Guid? EmployeeId, string? EmployeeName, string CredentialCode,
    Guid? AccessPointId, string? AccessPointName, string Movement, string Result,
    string? RejectionReason, DateTime OccurredAt);

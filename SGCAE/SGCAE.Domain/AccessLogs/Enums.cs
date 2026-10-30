namespace SGCAE.Domain.AccessLogs;

public enum MovementType { Entry, Exit }

public enum AccessResult { Authorized, Rejected }

public enum RejectionReason
{
    AccessPointNotFound,
    CredentialNotFound,
    CredentialRevoked,
    CredentialBlocked,
    EmployeeInactive,
    AccessPointDisabled
}

using SGCAE.Domain.AccessLogs;
using SGCAE.Domain.AccessPoints;
using SGCAE.Domain.Common;
using SGCAE.Domain.Credentials;
using SGCAE.Domain.Employees;

namespace SGCAE.Domain.Tests;

public class EmployeeTests
{
    private static Employee NewEmployee() =>
        Employee.Create("1001", "Ana", "Pérez", "Ana@Empresa.com", "Operaria", "Bodega");

    [Fact]
    public void Create_ValidData_IsActiveAndEmailNormalized()
    {
        var employee = NewEmployee();

        Assert.True(employee.IsActive);
        Assert.Equal("ana@empresa.com", employee.Email.Value);
    }

    [Fact]
    public void Create_InvalidEmail_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Employee.Create("1001", "Ana", "Pérez", "correo-invalido", "Operaria", "Bodega"));
    }

    [Fact]
    public void Create_EmptyName_Throws()
    {
        Assert.Throws<DomainException>(() =>
            Employee.Create("1001", " ", "Pérez", "ana@empresa.com", "Operaria", "Bodega"));
    }

    [Fact]
    public void Deactivate_Twice_Throws()
    {
        var employee = NewEmployee();
        employee.Deactivate();

        Assert.False(employee.IsActive);
        Assert.Throws<DomainException>(() => employee.Deactivate());
    }
}

public class CredentialTests
{
    private static Employee NewEmployee() =>
        Employee.Create("1001", "Ana", "Pérez", "ana@empresa.com", "Operaria", "Bodega");

    [Fact]
    public void Issue_ForInactiveEmployee_Throws()
    {
        var employee = NewEmployee();
        employee.Deactivate();

        Assert.Throws<DomainException>(() => Credential.Issue(employee, "CRD-001"));
    }

    [Fact]
    public void BlockAndReactivate_Works()
    {
        var credential = Credential.Issue(NewEmployee(), "CRD-001");

        credential.Block();
        Assert.False(credential.CanBeUsed);

        credential.Reactivate();
        Assert.True(credential.CanBeUsed);
    }

    [Fact]
    public void Revoked_CannotBeReactivatedNorBlocked()
    {
        var credential = Credential.Issue(NewEmployee(), "CRD-001");
        credential.Revoke();

        Assert.Throws<DomainException>(() => credential.Reactivate());
        Assert.Throws<DomainException>(() => credential.Block());
    }
}

public class AccessLogTests
{
    private static Employee NewEmployee() =>
        Employee.Create("1001", "Ana", "Pérez", "ana@empresa.com", "Operaria", "Bodega");

    [Fact]
    public void Register_AllValid_IsAuthorized()
    {
        var employee = NewEmployee();
        var credential = Credential.Issue(employee, "CRD-001");
        var point = AccessPoint.Create("Recepción", null);

        var log = AccessLog.Register("CRD-001", MovementType.Entry, credential, employee, point);

        Assert.Equal(AccessResult.Authorized, log.Result);
        Assert.Null(log.Reason);
    }

    [Fact]
    public void Register_UnknownCredential_IsRejected()
    {
        var point = AccessPoint.Create("Recepción", null);

        var log = AccessLog.Register("XXX", MovementType.Entry, null, null, point);

        Assert.Equal(AccessResult.Rejected, log.Result);
        Assert.Equal(RejectionReason.CredentialNotFound, log.Reason);
    }

    [Fact]
    public void Register_BlockedCredential_IsRejected()
    {
        var employee = NewEmployee();
        var credential = Credential.Issue(employee, "CRD-001");
        credential.Block();
        var point = AccessPoint.Create("Recepción", null);

        var log = AccessLog.Register("CRD-001", MovementType.Entry, credential, employee, point);

        Assert.Equal(RejectionReason.CredentialBlocked, log.Reason);
    }

    [Fact]
    public void Register_InactiveEmployee_IsRejected()
    {
        var employee = NewEmployee();
        var credential = Credential.Issue(employee, "CRD-001");
        employee.Deactivate();
        var point = AccessPoint.Create("Recepción", null);

        var log = AccessLog.Register("CRD-001", MovementType.Exit, credential, employee, point);

        Assert.Equal(AccessResult.Rejected, log.Result);
        Assert.Equal(RejectionReason.EmployeeInactive, log.Reason);
    }

    [Fact]
    public void Register_DisabledAccessPoint_IsRejected()
    {
        var employee = NewEmployee();
        var credential = Credential.Issue(employee, "CRD-001");
        var point = AccessPoint.Create("Bodega", null);
        point.Disable();

        var log = AccessLog.Register("CRD-001", MovementType.Entry, credential, employee, point);

        Assert.Equal(RejectionReason.AccessPointDisabled, log.Reason);
    }
}

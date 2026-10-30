using SGCAE.Domain.Common;

namespace SGCAE.Domain.Employees;

public sealed class Employee : Entity
{
    public string Identification { get; private set; } = default!;
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public Email Email { get; private set; } = default!;
    public string Position { get; private set; } = default!;
    public string Area { get; private set; } = default!;
    public EmployeeStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public bool IsActive => Status == EmployeeStatus.Active;

    private Employee() { } // requerido por EF Core

    public static Employee Create(string identification, string firstName, string lastName,
        string email, string position, string area)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            Identification = Guard.Required(identification, "La identificación", 30),
            Status = EmployeeStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        employee.SetProfile(firstName, lastName, email, position, area);
        return employee;
    }

    /// <summary>La identificación no se puede cambiar; el resto de datos sí.</summary>
    public void Update(string firstName, string lastName, string email, string position, string area)
        => SetProfile(firstName, lastName, email, position, area);

    public void Deactivate()
    {
        if (!IsActive)
            throw new DomainException("El empleado ya se encuentra inactivo.");
        Status = EmployeeStatus.Inactive;
    }

    private void SetProfile(string firstName, string lastName, string email, string position, string area)
    {
        FirstName = Guard.Required(firstName, "Los nombres", 100);
        LastName = Guard.Required(lastName, "Los apellidos", 100);
        Email = Email.Create(email);
        Position = Guard.Required(position, "El cargo", 100);
        Area = Guard.Required(area, "El área organizacional", 100);
    }
}

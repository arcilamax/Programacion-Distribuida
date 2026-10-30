namespace SGCAE.Application.Employees;

public sealed record EmployeeDto(Guid Id, string Identification, string FirstName, string LastName,
    string Email, string Position, string Area, string Status, DateTime CreatedAt);

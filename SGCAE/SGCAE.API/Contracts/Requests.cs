namespace SGCAE.API.Contracts;

public sealed record UpdateEmployeeRequest(string FirstName, string LastName, string Email,
    string Position, string Area);

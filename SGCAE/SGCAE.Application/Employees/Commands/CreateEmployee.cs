using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;
using SGCAE.Domain.Employees;

namespace SGCAE.Application.Employees.Commands;

public sealed record CreateEmployeeCommand(string Identification, string FirstName, string LastName,
    string Email, string Position, string Area) : IRequest<Guid>;

public sealed class CreateEmployeeValidator : AbstractValidator<CreateEmployeeCommand>
{
    public CreateEmployeeValidator()
    {
        RuleFor(x => x.Identification).NotEmpty().MaximumLength(30);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Area).NotEmpty().MaximumLength(100);
    }
}

public sealed class CreateEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : IRequestHandler<CreateEmployeeCommand, Guid>
{
    public async Task<Guid> Handle(CreateEmployeeCommand request, CancellationToken ct)
    {
        var employee = Employee.Create(request.Identification, request.FirstName, request.LastName,
            request.Email, request.Position, request.Area);

        if (await employees.ExistsByIdentificationAsync(employee.Identification, ct))
            throw new ConflictException($"Ya existe un empleado con la identificación '{employee.Identification}'.");

        if (await employees.ExistsByEmailAsync(employee.Email, null, ct))
            throw new ConflictException($"Ya existe un empleado con el correo '{employee.Email}'.");

        await employees.AddAsync(employee, ct);
        await uow.SaveChangesAsync(ct);
        return employee.Id;
    }
}

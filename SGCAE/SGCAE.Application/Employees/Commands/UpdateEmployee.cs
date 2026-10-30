using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;

namespace SGCAE.Application.Employees.Commands;

public sealed record UpdateEmployeeCommand(Guid Id, string FirstName, string LastName,
    string Email, string Position, string Area) : IRequest;

public sealed class UpdateEmployeeValidator : AbstractValidator<UpdateEmployeeCommand>
{
    public UpdateEmployeeValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Email).NotEmpty().MaximumLength(200).EmailAddress();
        RuleFor(x => x.Position).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Area).NotEmpty().MaximumLength(100);
    }
}

public sealed class UpdateEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : IRequestHandler<UpdateEmployeeCommand>
{
    public async Task Handle(UpdateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Empleado", request.Id);

        employee.Update(request.FirstName, request.LastName, request.Email, request.Position, request.Area);

        if (await employees.ExistsByEmailAsync(employee.Email, employee.Id, ct))
            throw new ConflictException($"Ya existe otro empleado con el correo '{employee.Email}'.");

        await uow.SaveChangesAsync(ct);
    }
}

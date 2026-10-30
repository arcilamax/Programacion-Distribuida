using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common.Exceptions;

namespace SGCAE.Application.Employees.Commands;

public sealed record DeactivateEmployeeCommand(Guid Id) : IRequest;

public sealed class DeactivateEmployeeHandler(IEmployeeRepository employees, IUnitOfWork uow)
    : IRequestHandler<DeactivateEmployeeCommand>
{
    public async Task Handle(DeactivateEmployeeCommand request, CancellationToken ct)
    {
        var employee = await employees.GetByIdAsync(request.Id, ct)
            ?? throw new NotFoundException("Empleado", request.Id);

        employee.Deactivate();
        await uow.SaveChangesAsync(ct);
    }
}

using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Application.Common.Exceptions;
using SGCAE.Domain.Employees;

namespace SGCAE.Application.Employees.Queries;

public sealed record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto>;

public sealed class GetEmployeeByIdHandler(IEmployeeQueries queries)
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto>
{
    public async Task<EmployeeDto> Handle(GetEmployeeByIdQuery request, CancellationToken ct)
        => await queries.GetByIdAsync(request.Id, ct)
           ?? throw new NotFoundException("Empleado", request.Id);
}

public sealed record GetEmployeesQuery(EmployeeStatus? Status, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<EmployeeDto>>;

public sealed class GetEmployeesValidator : AbstractValidator<GetEmployeesQuery>
{
    public GetEmployeesValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

public sealed class GetEmployeesHandler(IEmployeeQueries queries)
    : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    public Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken ct)
        => queries.SearchAsync(request.Status, request.Page, request.PageSize, ct);
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using SGCAE.API.Contracts;
using SGCAE.Application.Common;
using SGCAE.Application.Employees;
using SGCAE.Application.Employees.Commands;
using SGCAE.Application.Employees.Queries;
using SGCAE.Domain.Employees;

namespace SGCAE.API.Controllers;

[ApiController]
[Route("api/employees")]
public sealed class EmployeesController(ISender sender) : ControllerBase
{
    /// <summary>RF-01: registra un empleado.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateEmployeeCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        var employee = await sender.Send(new GetEmployeeByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, employee);
    }

    /// <summary>RF-01: consulta un empleado por su Id.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EmployeeDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetEmployeeByIdQuery(id), ct));

    /// <summary>RF-01: lista empleados (paginado), con filtro opcional por estado.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EmployeeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Search([FromQuery] EmployeeStatus? status, [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => Ok(await sender.Send(new GetEmployeesQuery(status, page, pageSize), ct));

    /// <summary>RF-01: actualiza los datos de un empleado.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateEmployeeRequest request, CancellationToken ct)
    {
        await sender.Send(new UpdateEmployeeCommand(id, request.FirstName, request.LastName,
            request.Email, request.Position, request.Area), ct);
        return NoContent();
    }

    /// <summary>RF-01: desactiva un empleado (su vínculo laboral queda inactivo).</summary>
    [HttpPost("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        await sender.Send(new DeactivateEmployeeCommand(id), ct);
        return NoContent();
    }
}

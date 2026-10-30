using MediatR;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.Credentials;
using SGCAE.Application.Credentials.Commands;
using SGCAE.Application.Credentials.Queries;

namespace SGCAE.API.Controllers;

[ApiController]
[Route("api/credentials")]
public sealed class CredentialsController(ISender sender) : ControllerBase
{
    /// <summary>RF-02: asigna una credencial a un empleado.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Assign(AssignCredentialCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        var credential = await sender.Send(new GetCredentialByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, credential);
    }

    /// <summary>RF-02: consulta una credencial y su estado.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CredentialDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetCredentialByIdQuery(id), ct));

    /// <summary>RF-02: lista las credenciales de un empleado.</summary>
    [HttpGet("/api/employees/{employeeId:guid}/credentials")]
    [ProducesResponseType(typeof(IReadOnlyList<CredentialDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByEmployee(Guid employeeId, CancellationToken ct)
        => Ok(await sender.Send(new GetCredentialsByEmployeeQuery(employeeId), ct));

    /// <summary>RF-02: bloquea una credencial (temporal, se puede reactivar).</summary>
    [HttpPost("{id:guid}/block")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Block(Guid id, CancellationToken ct)
    {
        await sender.Send(new BlockCredentialCommand(id), ct);
        return NoContent();
    }

    /// <summary>Reactiva una credencial bloqueada.</summary>
    [HttpPost("{id:guid}/reactivate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Reactivate(Guid id, CancellationToken ct)
    {
        await sender.Send(new ReactivateCredentialCommand(id), ct);
        return NoContent();
    }

    /// <summary>RF-02: revoca una credencial (definitivo, no se puede deshacer).</summary>
    [HttpPost("{id:guid}/revoke")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Revoke(Guid id, CancellationToken ct)
    {
        await sender.Send(new RevokeCredentialCommand(id), ct);
        return NoContent();
    }
}

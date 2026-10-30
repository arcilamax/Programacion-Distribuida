using MediatR;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.AccessPoints;
using SGCAE.Application.AccessPoints.Commands;
using SGCAE.Application.AccessPoints.Queries;

namespace SGCAE.API.Controllers;

[ApiController]
[Route("api/access-points")]
public sealed class AccessPointsController(ISender sender) : ControllerBase
{
    /// <summary>RF-06: registra un punto de acceso.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccessPointDto), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateAccessPointCommand command, CancellationToken ct)
    {
        var id = await sender.Send(command, ct);
        var point = await sender.Send(new GetAccessPointByIdQuery(id), ct);
        return CreatedAtAction(nameof(GetById), new { id }, point);
    }

    /// <summary>RF-06: lista los puntos de acceso (opcionalmente solo los habilitados).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccessPointDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool onlyEnabled = false, CancellationToken ct = default)
        => Ok(await sender.Send(new GetAccessPointsQuery(onlyEnabled), ct));

    /// <summary>RF-06: consulta un punto de acceso.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AccessPointDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        => Ok(await sender.Send(new GetAccessPointByIdQuery(id), ct));

    /// <summary>Habilita un punto de acceso.</summary>
    [HttpPost("{id:guid}/enable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Enable(Guid id, CancellationToken ct)
    {
        await sender.Send(new EnableAccessPointCommand(id), ct);
        return NoContent();
    }

    /// <summary>Deshabilita un punto de acceso: todo acceso quedará rechazado.</summary>
    [HttpPost("{id:guid}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Disable(Guid id, CancellationToken ct)
    {
        await sender.Send(new DisableAccessPointCommand(id), ct);
        return NoContent();
    }
}

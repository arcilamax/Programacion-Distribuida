using MediatR;
using Microsoft.AspNetCore.Mvc;
using SGCAE.Application.AccessLogs;
using SGCAE.Application.AccessLogs.Commands;
using SGCAE.Application.AccessLogs.Queries;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessLogs;

namespace SGCAE.API.Controllers;

[ApiController]
[Route("api/access")]
public sealed class AccessController(ISender sender) : ControllerBase
{
    /// <summary>
    /// RF-03: registra un intento de entrada o salida.
    /// Siempre responde 200 con el resultado (Authorized o Rejected, con su motivo) y deja el registro guardado.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccessLogDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Register(RegisterAccessCommand command, CancellationToken ct)
        => Ok(await sender.Send(command, ct));

    /// <summary>RF-04: historial de accesos con filtros y paginación.</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(PagedResult<AccessLogDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> History(
        [FromQuery] Guid? employeeId,
        [FromQuery(Name = "from")] DateTime? fromDate,
        [FromQuery(Name = "to")] DateTime? toDate,
        [FromQuery] Guid? accessPointId,
        [FromQuery] AccessResult? result,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => Ok(await sender.Send(
            new SearchAccessLogsQuery(employeeId, fromDate, toDate, accessPointId, result, page, pageSize), ct));
}

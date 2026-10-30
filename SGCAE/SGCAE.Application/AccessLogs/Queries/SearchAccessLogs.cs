using FluentValidation;
using MediatR;
using SGCAE.Application.Abstractions;
using SGCAE.Application.Common;
using SGCAE.Domain.AccessLogs;

namespace SGCAE.Application.AccessLogs.Queries;

public sealed record SearchAccessLogsQuery(Guid? EmployeeId, DateTime? From, DateTime? To,
    Guid? AccessPointId, AccessResult? Result, int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<AccessLogDto>>;

public sealed class SearchAccessLogsValidator : AbstractValidator<SearchAccessLogsQuery>
{
    public SearchAccessLogsValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x).Must(x => !x.From.HasValue || !x.To.HasValue || x.From <= x.To)
            .WithMessage("La fecha inicial no puede ser mayor que la fecha final.");
    }
}

public sealed class SearchAccessLogsHandler(IAccessLogQueries queries)
    : IRequestHandler<SearchAccessLogsQuery, PagedResult<AccessLogDto>>
{
    public Task<PagedResult<AccessLogDto>> Handle(SearchAccessLogsQuery request, CancellationToken ct)
        => queries.SearchAsync(new AccessLogFilter(request.EmployeeId, request.From, request.To,
            request.AccessPointId, request.Result, request.Page, request.PageSize), ct);
}

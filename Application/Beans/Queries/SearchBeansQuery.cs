using AllTheBeans.Application.Beans.Dtos;
using AllTheBeans.Application.Common;
using MediatR;

namespace AllTheBeans.Application.Beans.Queries;

public sealed record SearchBeansQuery(
    string? Query = null,
    string? Country = null,
    string? Colour = null,
    decimal? MinCost = null,
    decimal? MaxCost = null,
    int Page = 1,
    int PageSize = 20) : IRequest<IReadOnlyList<BeanDto>>;

public sealed class SearchBeansHandler(IBeanRepository repo)
    : IRequestHandler<SearchBeansQuery, IReadOnlyList<BeanDto>>
{
    public async Task<IReadOnlyList<BeanDto>> Handle(SearchBeansQuery request, CancellationToken ct)
    {
        var criteria = new BeanSearchCriteria(
            request.Query, request.Country, request.Colour,
            request.MinCost, request.MaxCost, request.Page, request.PageSize);
        var beans = await repo.SearchAsync(criteria, ct);
        return beans.Select(BeanDto.From).ToList();
    }
}

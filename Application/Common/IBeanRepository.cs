using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Common;

public interface IBeanRepository
{
    Task<Bean?> GetAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<Bean>> SearchAsync(BeanSearchCriteria criteria, CancellationToken ct);
    Task AddAsync(Bean bean, CancellationToken ct);
    Task UpdateAsync(Bean bean, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
    Task<int> CountAsync(CancellationToken ct);
    Task<Bean?> GetRandomExcludingAsync(Guid? excludeId, CancellationToken ct);
}

public sealed record BeanSearchCriteria(
    string? Query,
    string? Country,
    string? Colour,
    decimal? MinCost,
    decimal? MaxCost,
    int Page = 1,
    int PageSize = 20);

using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Common;

public interface IDailyBeanRepository
{
    Task<DailyBeanSelection?> GetForDateAsync(DateOnly date, CancellationToken ct);
    Task<DailyBeanSelection?> GetMostRecentAsync(CancellationToken ct);
    Task AddAsync(DailyBeanSelection selection, CancellationToken ct);
}

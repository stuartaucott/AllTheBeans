using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Persistence.Repositories;

public sealed class DailyBeanRepository(BeansDbContext db) : IDailyBeanRepository
{
    public Task<DailyBeanSelection?> GetForDateAsync(DateOnly date, CancellationToken ct) =>
        db.DailyBeanSelections.AsNoTracking().FirstOrDefaultAsync(x => x.Date == date, ct);

    public Task<DailyBeanSelection?> GetMostRecentAsync(CancellationToken ct) =>
        db.DailyBeanSelections.AsNoTracking().OrderByDescending(x => x.Date).FirstOrDefaultAsync(ct);

    public async Task AddAsync(DailyBeanSelection selection, CancellationToken ct) =>
        await db.DailyBeanSelections.AddAsync(selection, ct);
}

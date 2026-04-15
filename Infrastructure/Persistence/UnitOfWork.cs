using AllTheBeans.Application.Common;

namespace AllTheBeans.Infrastructure.Persistence;

public sealed class UnitOfWork(BeansDbContext db) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}

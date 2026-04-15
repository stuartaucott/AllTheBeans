using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Persistence.Repositories;

public sealed class BeanRepository(BeansDbContext db) : IBeanRepository
{
    public Task<Bean?> GetAsync(Guid id, CancellationToken ct) =>
        db.Beans.FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<IReadOnlyList<Bean>> SearchAsync(BeanSearchCriteria c, CancellationToken ct)
    {
        IQueryable<Bean> q = db.Beans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(c.Query))
        {
            var term = $"%{c.Query.Trim()}%";
            q = q.Where(b => EF.Functions.Like(b.Name, term) || EF.Functions.Like(b.Description, term));
        }
        if (!string.IsNullOrWhiteSpace(c.Country)) q = q.Where(b => b.Country == c.Country);
        if (!string.IsNullOrWhiteSpace(c.Colour)) q = q.Where(b => b.Colour == c.Colour);
        if (c.MinCost.HasValue) q = q.Where(b => b.Cost >= c.MinCost);
        if (c.MaxCost.HasValue) q = q.Where(b => b.Cost <= c.MaxCost);

        var page = Math.Max(1, c.Page);
        var size = Math.Clamp(c.PageSize, 1, 100);
        return await q.OrderBy(b => b.Name).Skip((page - 1) * size).Take(size).ToListAsync(ct);
    }

    public async Task AddAsync(Bean bean, CancellationToken ct) => await db.Beans.AddAsync(bean, ct);

    public Task UpdateAsync(Bean bean, CancellationToken ct)
    {
        db.Beans.Update(bean);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Beans.FirstOrDefaultAsync(b => b.Id == id, ct);
        if (entity is not null) db.Beans.Remove(entity);
    }

    public Task<int> CountAsync(CancellationToken ct) => db.Beans.CountAsync(ct);

    public async Task<Bean?> GetRandomExcludingAsync(Guid? excludeId, CancellationToken ct)
    {
        IQueryable<Bean> q = db.Beans.AsNoTracking();
        if (excludeId.HasValue) q = q.Where(b => b.Id != excludeId.Value);
        return await q.OrderBy(_ => EF.Functions.Random()).FirstOrDefaultAsync(ct);
    }
}

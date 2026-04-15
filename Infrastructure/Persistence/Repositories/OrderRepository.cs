using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Infrastructure.Persistence.Repositories;

public sealed class OrderRepository(BeansDbContext db) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken ct) => await db.Orders.AddAsync(order, ct);
}

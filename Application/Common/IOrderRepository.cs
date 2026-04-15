using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Common;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken ct);
}

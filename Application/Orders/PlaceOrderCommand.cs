using AllTheBeans.Application.Common;
using AllTheBeans.Domain.Entities;
using FluentValidation;
using MediatR;

namespace AllTheBeans.Application.Orders;

public sealed record PlaceOrderCommand(
    string CustomerName,
    string CustomerEmail,
    string ShippingAddress,
    Guid BeanId,
    int Quantity) : IRequest<Guid>;

public sealed class PlaceOrderValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderValidator()
    {
        RuleFor(x => x.CustomerName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CustomerEmail).NotEmpty().EmailAddress();
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.BeanId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).LessThanOrEqualTo(1000);
    }
}

public sealed class PlaceOrderHandler(
    IBeanRepository beans,
    IOrderRepository orders,
    IDateTimeProvider clock,
    IUnitOfWork uow) : IRequestHandler<PlaceOrderCommand, Guid>
{
    public async Task<Guid> Handle(PlaceOrderCommand request, CancellationToken ct)
    {
        var bean = await beans.GetAsync(request.BeanId, ct)
            ?? throw new KeyNotFoundException($"Bean {request.BeanId} not found.");

        var order = new Order(request.CustomerName, request.CustomerEmail,
                              request.ShippingAddress, bean.Id, request.Quantity,
                              bean.Cost, clock.UtcNow);
        await orders.AddAsync(order, ct);
        await uow.SaveChangesAsync(ct);
        return order.Id;
    }
}
namespace AllTheBeans.Domain.Entities;

public sealed class Order
{
    public Guid Id { get; private set; }
    public string CustomerName { get; private set; } = default!;
    public string CustomerEmail { get; private set; } = default!;
    public string ShippingAddress { get; private set; } = default!;
    public Guid BeanId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Total => UnitPrice * Quantity;
    public DateTimeOffset PlacedAt { get; private set; }

    private Order() { } // EF Core

    public Order(string customerName,
                 string customerEmail,
                 string shippingAddress,
                 Guid beanId,
                 int quantity,
                 decimal unitPrice,
                 DateTimeOffset placedAt)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new ArgumentException("Name required", nameof(customerName));
        if (string.IsNullOrWhiteSpace(customerEmail))
            throw new ArgumentException("Email required", nameof(customerEmail));
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity));
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice));

        Id = Guid.NewGuid();
        CustomerName = customerName;
        CustomerEmail = customerEmail;
        ShippingAddress = shippingAddress ?? string.Empty;
        BeanId = beanId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        PlacedAt = placedAt;
    }
}

using AllTheBeans.Application.Orders;
using FluentAssertions;

namespace AllTheBeans.Tests.Unit;

public class PlaceOrderValidatorTests
{
    private readonly PlaceOrderValidator _validator = new();

    [Fact]
    public void Valid_order_passes()
    {
        var cmd = new PlaceOrderCommand("Stuart", "stuart@test.com", "123 Street", Guid.NewGuid(), 2);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_email_fails()
    {
        var cmd = new PlaceOrderCommand("Stuart", "", "123 Street", Guid.NewGuid(), 2);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerEmail");
    }

    [Fact]
    public void Zero_quantity_fails()
    {
        var cmd = new PlaceOrderCommand("Stuart", "stuart@test.com", "123 Street", Guid.NewGuid(), 0);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Empty_bean_id_fails()
    {
        var cmd = new PlaceOrderCommand("Stuart", "stuart@test.com", "123 Street", Guid.Empty, 2);
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BeanId");
    }
}
using AllTheBeans.Application.Beans.Commands;
using FluentAssertions;

namespace AllTheBeans.Tests.Unit;

public class CreateBeanValidatorTests
{
    private readonly CreateBeanValidator _validator = new();

    [Fact]
    public void Valid_input_passes()
    {
        var cmd = new CreateBeanCommand("Kenya AA", "Bright citrus", "Kenya", "Medium", 14.5m, "http://x/y.png");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Empty_name_fails()
    {
        var cmd = new CreateBeanCommand("", "desc", "UK", "Dark", 10m, "");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Negative_cost_fails()
    {
        var cmd = new CreateBeanCommand("Test", "desc", "UK", "Dark", -5m, "");
        var result = _validator.Validate(cmd);
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Cost");
    }
}
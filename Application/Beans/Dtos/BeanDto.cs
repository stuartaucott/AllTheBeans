using AllTheBeans.Domain.Entities;

namespace AllTheBeans.Application.Beans.Dtos;

public sealed record BeanDto(
    Guid Id,
    string Name,
    string Description,
    string Country,
    string Colour,
    decimal Cost,
    string ImageUrl,
    bool IsBeanOfTheDay)
{
    public static BeanDto From(Bean b) =>
        new(b.Id, b.Name, b.Description, b.Country, b.Colour, b.Cost, b.ImageUrl, b.IsBeanOfTheDay);
}

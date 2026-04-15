namespace AllTheBeans.Domain.Entities;

public sealed class Bean
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string Country { get; private set; } = default!;
    public string Colour { get; private set; } = default!;
    public decimal Cost { get; private set; }
    public string ImageUrl { get; private set; } = default!;
    public bool IsBeanOfTheDay { get; private set; }

    // EF Core
    private Bean() { }

    public Bean(Guid id, string name, string description, string country,
                string colour, decimal cost, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost));
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Name = name;
        Description = description ?? string.Empty;
        Country = country ?? string.Empty;
        Colour = colour ?? string.Empty;
        Cost = cost;
        ImageUrl = imageUrl ?? string.Empty;
    }

    public void Update(string name, string description, string country,
                       string colour, decimal cost, string imageUrl)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required", nameof(name));
        if (cost < 0) throw new ArgumentOutOfRangeException(nameof(cost));
        Name = name;
        Description = description ?? string.Empty;
        Country = country ?? string.Empty;
        Colour = colour ?? string.Empty;
        Cost = cost;
        ImageUrl = imageUrl ?? string.Empty;
    }

    internal void MarkAsBeanOfTheDay(bool value) => IsBeanOfTheDay = value;
}
using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.Json;

namespace AllTheBeans.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task SeedAsync(BeansDbContext db, IHostEnvironment env, ILogger logger, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Beans.AnyAsync(ct))
        {
            logger.LogInformation("Beans table already populated; skipping seed.");
            return;
        }

        var path = Path.Combine(env.ContentRootPath, "allthebeans.json");
        if (!File.Exists(path))
        {
            logger.LogWarning("allthebeans.json not found at {Path}; nothing to seed.", path);
            return;
        }

        await using var stream = File.OpenRead(path);
        var seeds = await JsonSerializer.DeserializeAsync<List<BeanSeed>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }, ct)
            ?? [];

        foreach (var s in seeds)
        {
            var bean = new Bean(
                id: Guid.NewGuid(),
                name: s.Name ?? "",
                description: (s.Description ?? "").Trim(),
                country: s.Country ?? "",
                colour: s.Colour ?? "",
                cost: ParseCost(s.Cost),
                imageUrl: s.Image ?? "");
            await db.Beans.AddAsync(bean, ct);
        }

        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} beans.", seeds.Count);
    }

    private static decimal ParseCost(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return 0m;
        var cleaned = new string(raw.Where(c => char.IsDigit(c) || c == '.' || c == '-').ToArray());
        return decimal.TryParse(cleaned, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) ? v : 0m;
    }

    // Matches the JSON field names exactly.
    private sealed record BeanSeed(
        string? Name,
        string? Description,
        string? Country,
        string? Colour,
        string? Cost,
        string? Image);
}

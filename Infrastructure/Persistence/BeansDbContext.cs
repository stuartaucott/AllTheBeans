using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllTheBeans.Infrastructure.Persistence;

public sealed class BeansDbContext(DbContextOptions<BeansDbContext> options) : DbContext(options)
{
    public DbSet<Bean> Beans => Set<Bean>();
    public DbSet<DailyBeanSelection> DailyBeanSelections => Set<DailyBeanSelection>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BeansDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

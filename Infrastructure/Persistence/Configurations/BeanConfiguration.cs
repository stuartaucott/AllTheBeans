using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public sealed class BeanConfiguration : IEntityTypeConfiguration<Bean>
{
    public void Configure(EntityTypeBuilder<Bean> b)
    {
        b.ToTable("Beans");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).IsRequired().HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(4000);
        b.Property(x => x.Country).HasMaxLength(100);
        b.Property(x => x.Colour).HasMaxLength(50);
        b.Property(x => x.Cost).HasColumnType("decimal(10,2)");
        b.Property(x => x.ImageUrl).HasMaxLength(1000);
        b.HasIndex(x => x.Name);
        b.HasIndex(x => x.Country);
        b.HasIndex(x => x.Colour);
    }
}

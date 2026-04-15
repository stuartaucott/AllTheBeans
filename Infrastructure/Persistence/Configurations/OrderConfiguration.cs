using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");
        b.HasKey(x => x.Id);
        b.Property(x => x.CustomerName).IsRequired().HasMaxLength(200);
        b.Property(x => x.CustomerEmail).IsRequired().HasMaxLength(320);
        b.Property(x => x.ShippingAddress).IsRequired().HasMaxLength(500);
        b.Property(x => x.UnitPrice).HasColumnType("decimal(10,2)");
        b.Ignore(x => x.Total);
        b.HasOne<Bean>().WithMany().HasForeignKey(x => x.BeanId).OnDelete(DeleteBehavior.Restrict);
    }
}

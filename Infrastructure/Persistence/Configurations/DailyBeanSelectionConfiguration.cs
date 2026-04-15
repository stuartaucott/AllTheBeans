using AllTheBeans.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AllTheBeans.Infrastructure.Persistence.Configurations;

public sealed class DailyBeanSelectionConfiguration : IEntityTypeConfiguration<DailyBeanSelection>
{
    public void Configure(EntityTypeBuilder<DailyBeanSelection> b)
    {
        b.ToTable("DailyBeanSelections");
        b.HasKey(x => x.Date);
        b.Property(x => x.BeanId).IsRequired();
        b.HasOne<Bean>().WithMany().HasForeignKey(x => x.BeanId).OnDelete(DeleteBehavior.Restrict);
    }
}

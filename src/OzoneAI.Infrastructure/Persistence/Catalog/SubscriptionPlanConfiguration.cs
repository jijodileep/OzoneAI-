using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OzoneAI.Domain.Catalog;

namespace OzoneAI.Infrastructure.Persistence.Catalog;

internal sealed class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
    {
        builder.ToTable("subscription_plans");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Details).HasMaxLength(4000);
        builder.Property(x => x.ModuleFlagsJson).HasMaxLength(4000).IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
    }
}

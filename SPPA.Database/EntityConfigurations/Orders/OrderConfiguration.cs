using SPPA.Database.Extensions;
using SPPA.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Orders;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Name).IsRequired();

        builder.Property(x => x.CustomFields)
               .HasJsonConversion()
               .IsRequired(false)
               .HasDefaultValue(null);

        builder.HasOne(x => x.Workspace)
               .WithMany(x => x.Orders)
               .HasForeignKey(x => x.WorkspaceId)
               .IsRequired();
    }
}

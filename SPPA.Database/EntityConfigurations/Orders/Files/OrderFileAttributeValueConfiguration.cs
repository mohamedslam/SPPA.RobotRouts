using SPPA.Domain.Entities.Orders.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Orders.Files;

public class OrderFileAttributeValueConfiguration : IEntityTypeConfiguration<OrderFileAttributeValue>
{
    public void Configure(EntityTypeBuilder<OrderFileAttributeValue> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Value).IsRequired();

        builder.HasOne(x => x.Order)
               .WithMany()
               .HasForeignKey(x => x.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Attribute)
               .WithMany()
               .HasForeignKey(x => x.AttributeId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}

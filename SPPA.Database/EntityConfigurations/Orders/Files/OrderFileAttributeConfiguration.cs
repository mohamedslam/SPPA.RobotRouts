using SPPA.Domain.Entities.Orders.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Orders.Files;

public class OrderFileAttributeConfiguration : IEntityTypeConfiguration<OrderFileAttribute>
{
    public void Configure(EntityTypeBuilder<OrderFileAttribute> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Key).IsRequired();

        builder.Property(x => x.Label).IsRequired();

        builder.HasOne(x => x.Order)
               .WithMany()
               .HasForeignKey(x => x.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}

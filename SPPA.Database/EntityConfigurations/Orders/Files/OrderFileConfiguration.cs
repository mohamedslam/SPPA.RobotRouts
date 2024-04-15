using SPPA.Domain.Entities.Orders.Files;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Orders.Files;

public class OrderFileConfiguration : IEntityTypeConfiguration<OrderFile>
{
    public void Configure(EntityTypeBuilder<OrderFile> builder)
    {
        //builder.ToTable("IfcFiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.FilePath).IsRequired();

        builder.Property(x => x.OriginalFileName).IsRequired();

        builder.HasOne(x => x.Order)
               .WithMany(x => x.OrderFiles)
               .HasForeignKey(x => x.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }
}

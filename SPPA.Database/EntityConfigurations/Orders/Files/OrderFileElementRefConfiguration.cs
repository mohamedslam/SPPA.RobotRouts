using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SPPA.Domain.Entities.Orders.Files;

namespace SPPA.Database.EntityConfigurations.Orders.Files;

public class OrderFileElementRefConfiguration : IEntityTypeConfiguration<OrderFileElementRef>
{
    public void Configure(EntityTypeBuilder<OrderFileElementRef> builder)
    {
        //builder.ToTable("IfcFiles");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Order)
               .WithMany()
               .HasForeignKey(x => x.OrderId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.File)
               .WithMany(x => x.ElementsRef)
               .HasForeignKey(x => x.FileId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);
    }

}

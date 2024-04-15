using SPPA.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Users;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new { x.UserId, x.WorkspaceId })
               .IsUnique()
               .HasFilter($"\"{nameof(UserRole.OrderId)}\" IS NULL");

        builder.HasIndex(x => new { x.UserId, x.OrderId })
               .IsUnique()
               .HasFilter($"\"{nameof(UserRole.WorkspaceId)}\" IS NULL");
        // TODO
        // .AreNullsDistinct();

        builder.HasOne(x => x.User)
               .WithMany(x => x.Roles)
               .HasForeignKey(x => x.UserId)
               .IsRequired()
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Workspace)
               .WithMany(x => x.Roles)
               .HasForeignKey(x => x.WorkspaceId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
               .WithMany(x => x.Roles)
               .HasForeignKey(x => x.OrderId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

    }
}

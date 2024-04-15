using SPPA.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Users;

public class UserInviteConfiguration : IEntityTypeConfiguration<UserInvite>
{
    public void Configure(EntityTypeBuilder<UserInvite> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => new { x.UserEmail, x.WorkspaceId })
               .IsUnique()
               .HasFilter($"\"{nameof(UserRole.OrderId)}\" IS NULL");

        builder.HasIndex(x => new { x.UserEmail, x.OrderId })
               .IsUnique()
               .HasFilter($"\"{nameof(UserRole.WorkspaceId)}\" IS NULL");
        // TODO
        // .AreNullsDistinct();

        builder.HasOne(x => x.Workspace)
               .WithMany(x => x.Invites)
               .HasForeignKey(x => x.WorkspaceId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Order)
               .WithMany(x => x.Invites)
               .HasForeignKey(x => x.OrderId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

    }
}

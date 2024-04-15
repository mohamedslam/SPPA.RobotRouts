using SPPA.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.HasIndex(x => x.UserName)
               .IsUnique();

        builder.HasIndex(x => x.Email)
               .IsUnique();

        builder.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
    }
}

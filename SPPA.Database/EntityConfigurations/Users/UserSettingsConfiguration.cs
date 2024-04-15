using SPPA.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SPPA.Database.EntityConfigurations.Users;

public class UserSettingsConfiguration : IEntityTypeConfiguration<UserSettings>
{
    public void Configure(EntityTypeBuilder<UserSettings> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Id).HasColumnName("UserId");

        builder.Property(x => x.InterfaceLanguage)
               .HasMaxLength(2)
               .IsFixedLength();

        builder.Property(x => x.ReportLanguage)
               .HasMaxLength(2)
               .IsFixedLength();

        builder.HasOne(x => x.User)
               .WithOne(x => x.UserSettings)
               .HasForeignKey<UserSettings>(x => x.Id)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Cascade);

    }
}

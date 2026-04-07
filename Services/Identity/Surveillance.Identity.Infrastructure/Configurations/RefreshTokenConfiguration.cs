using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Surveillance.Identity.Domain.Entities;

namespace Surveillance.Identity.Infrastructure.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Token).IsUnique();
            builder.HasIndex(x => x.UserId);

            builder.Property(x => x.Token).IsRequired().HasMaxLength(500);
            builder.Property(x => x.ExpiresAt).IsRequired();

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

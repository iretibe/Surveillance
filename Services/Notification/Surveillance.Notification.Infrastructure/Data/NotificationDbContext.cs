using Microsoft.EntityFrameworkCore;

namespace Surveillance.Notification.Infrastructure.Data
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options)
            : base(options) { }

        public DbSet<Domain.Entities.Notification> Notifications => Set<Domain.Entities.Notification>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("notification");

            builder.Entity<Domain.Entities.Notification>(e =>
            {
                e.HasKey(x => x.Id);
                e.Property(x => x.Message).IsRequired();
            });
        }
    }
}

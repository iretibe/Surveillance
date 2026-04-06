using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Surveillance.Notification.Infrastructure.Data
{
    internal class NotificationDbContextFactory : IDesignTimeDbContextFactory<NotificationDbContext>
    {
        public NotificationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NotificationDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Surveillance_DB;User Id=sa;Password=Admin@2025!;TrustServerCertificate=True;");

            return new NotificationDbContext(optionsBuilder.Options);
        }
    }
}

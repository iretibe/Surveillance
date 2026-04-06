using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Surveillance.Identity.Infrastructure.Data
{
    internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost,1433;Database=Surveillance_DB;User Id=sa;Password=Admin@2025!;TrustServerCertificate=True;");

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}

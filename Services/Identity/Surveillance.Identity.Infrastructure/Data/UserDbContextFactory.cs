using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Surveillance.Identity.Infrastructure.Data
{
    internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

            optionsBuilder.UseSqlServer("Server=localhost,1434;Database=Surveillance_DB;User Id=sa;Password=Somad@2026$;TrustServerCertificate=True;");

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}

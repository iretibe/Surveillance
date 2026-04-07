using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Surveillance.Alert.Infrastructure.Data
{
    internal class AlertDbContextFactory : IDesignTimeDbContextFactory<AlertDbContext>
    {
        public AlertDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AlertDbContext>();
            
            optionsBuilder.UseSqlServer("Server=localhost,1434;Database=Surveillance_DB;User Id=sa;Password=Somad@2026$;TrustServerCertificate=True;");

            return new AlertDbContext(optionsBuilder.Options);
        }
    }
}

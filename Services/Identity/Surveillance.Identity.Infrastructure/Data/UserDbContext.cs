using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Surveillance.Identity.Domain.Entities;

namespace Surveillance.Identity.Infrastructure.Data
{
    public class UserDbContext : IdentityDbContext<User>
    {
        public UserDbContext(DbContextOptions options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}

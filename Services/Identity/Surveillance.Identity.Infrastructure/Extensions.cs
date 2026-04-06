using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surveillance.Identity.Domain.Entities;
using Surveillance.Identity.Infrastructure.Data;

namespace Surveillance.Identity.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("Default")));

            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<UserDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}

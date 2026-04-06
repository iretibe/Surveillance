using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Surveillance.Alert.Domain.Repositories;
using Surveillance.Alert.Infrastructure.BackgroundJob;
using Surveillance.Alert.Infrastructure.Data;
using Surveillance.Alert.Infrastructure.Repositories;
using Surveillance.EventBus.Caching;
using Surveillance.EventBus.Events;
using Surveillance.Logging;
using Surveillance.SharedKernel;

namespace Surveillance.Alert.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AlertDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql =>
                    {
                        sql.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    }));

            services.AddScoped<IAlertRepository, AlertRepository>();

            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AlertDbContext>());

            services.AddSingleton<IEventBus, RabbitMqEventBus>();
            services.AddSingleton<IAppLogger, AppLogger>();

            services.AddHostedService<OutboxProcessor>();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = "redis:6379";
            });

            services.AddScoped<CacheService>();

            return services;
        }
    }
}

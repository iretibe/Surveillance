using Microsoft.Extensions.DependencyInjection;
using Surveillance.Notification.Application.BackgroundJob;
using Surveillance.Notification.Application.EventHandlers;

namespace Surveillance.Notification.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<NotificationEventHandler>();

            services.AddHostedService<RabbitConsumer>();

            return services;
        }
    }
}

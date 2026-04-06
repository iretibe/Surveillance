using Microsoft.Extensions.DependencyInjection;
using Surveillance.Alert.Application.Commands.Handlers;

namespace Surveillance.Alert.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Command handler registration
            services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateAlertCommandHandler>());

            return services;
        }
    }
}

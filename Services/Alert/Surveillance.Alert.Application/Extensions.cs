using Microsoft.Extensions.DependencyInjection;
using Surveillance.Alert.Application.Commands.Handlers;
using Surveillance.Alert.Application.Sagas;
using Surveillance.Saga;

namespace Surveillance.Alert.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Command handler registration
            services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<CreateAlertCommandHandler>());

            services.AddScoped<SagaDispatcher>();
            services.AddScoped<ISaga, AlertSaga>();

            return services;
        }
    }
}

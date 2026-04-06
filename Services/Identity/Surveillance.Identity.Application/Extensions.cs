using Microsoft.Extensions.DependencyInjection;
using Surveillance.Identity.Application.Commands.Handlers;

namespace Surveillance.Identity.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Command handler registration
            services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommandHandler>());

            return services;
        }
    }
}

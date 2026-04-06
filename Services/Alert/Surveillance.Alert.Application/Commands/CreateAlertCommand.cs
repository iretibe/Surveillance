using MediatR;

namespace Surveillance.Alert.Application.Commands
{
    public record CreateAlertCommand(string Message) : IRequest<Guid>;
}

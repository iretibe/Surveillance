using MediatR;

namespace Surveillance.Alert.Application.Commands
{
    public record DeleteAlertCommand(Guid Id) : IRequest;
}

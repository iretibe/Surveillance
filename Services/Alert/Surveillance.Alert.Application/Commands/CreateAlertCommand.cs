using MediatR;

namespace Surveillance.Alert.Application.Commands
{
    public record CreateAlertCommand(string Message, Guid UserId) : IRequest<Guid>;
}

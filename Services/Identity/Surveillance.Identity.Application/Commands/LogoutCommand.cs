using MediatR;

namespace Surveillance.Identity.Application.Commands
{
    public record LogoutCommand(Guid UserId) : IRequest;
}

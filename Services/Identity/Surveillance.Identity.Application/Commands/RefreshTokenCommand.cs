using MediatR;
using Surveillance.Identity.Application.Responses;

namespace Surveillance.Identity.Application.Commands
{
    public record RefreshTokenCommand(string RefreshToken) 
        : IRequest<RefreshTokenResponse>;
}

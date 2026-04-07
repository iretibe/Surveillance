using MediatR;
using Surveillance.Identity.Application.Responses;

namespace Surveillance.Identity.Application.Queries
{
    public record LoginQuery(string Username, string Password) 
        : IRequest<LoginResponse>;
}

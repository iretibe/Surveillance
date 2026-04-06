using MediatR;

namespace Surveillance.Identity.Application.Commands
{
    public record RegisterUserCommand(string Username,
        string Email, string Password, string FirstName,
        string LastName, string PhoneNumber) 
        : IRequest<Guid>;
}

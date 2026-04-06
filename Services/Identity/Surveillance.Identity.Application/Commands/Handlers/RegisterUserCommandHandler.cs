using MediatR;
using Microsoft.AspNetCore.Identity;
using Surveillance.Identity.Domain.Entities;

namespace Surveillance.Identity.Application.Commands.Handlers
{
    public class RegisterUserCommandHandler
        : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly UserManager<User> _userManager;

        public RegisterUserCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                throw new Exception(string.Join(",", result.Errors.Select(e => e.Description)));

            return Guid.Parse(user.Id);
        }
    }
}

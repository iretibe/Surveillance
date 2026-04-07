using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Surveillance.Identity.Application.Responses;
using Surveillance.Identity.Domain.Entities;
using Surveillance.Identity.Domain.Repositories;
using Surveillance.SharedKernel.Security;

namespace Surveillance.Identity.Application.Queries.Handlers
{
    public class LoginQueryHandler : IRequestHandler<LoginQuery, LoginResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly JwtSettings _jwtSettings;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginQueryHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            JwtTokenGenerator jwtGenerator,
            IOptions<JwtSettings> jwtSettings,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
            _jwtSettings = jwtSettings.Value;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> Handle(LoginQuery request, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtGenerator.GenerateToken(
                Guid.Parse(user.Id),
                user.UserName!,
                user.Email!,
                roles);

            var refreshToken = await _refreshTokenRepository.CreateAsync(Guid.Parse(user.Id));

            return new LoginResponse(
                token,
                refreshToken,
                DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                new UserInfo(
                    Guid.Parse(user.Id),
                    user.UserName!,
                    user.Email!,
                    user.FirstName ?? string.Empty,
                    user.LastName ?? string.Empty,
                    roles));
        }
    }
}

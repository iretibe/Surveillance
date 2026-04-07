using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Surveillance.Identity.Application.Responses;
using Surveillance.Identity.Domain.Entities;
using Surveillance.Identity.Domain.Repositories;
using Surveillance.SharedKernel.Security;

namespace Surveillance.Identity.Application.Commands.Handlers
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly UserManager<User> _userManager;
        private readonly JwtTokenGenerator _jwtGenerator;
        private readonly JwtSettings _jwtSettings;

        public RefreshTokenCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            UserManager<User> userManager,
            JwtTokenGenerator jwtGenerator,
            IOptions<JwtSettings> jwtSettings)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Validate refresh token
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (refreshToken == null || !refreshToken.IsActive)
                throw new UnauthorizedAccessException("Invalid or expired refresh token");

            // Get user
            var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            // Revoke old refresh token
            await _refreshTokenRepository.RevokeAsync(refreshToken.Id);

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Generate new tokens
            var newToken = _jwtGenerator.GenerateToken(
                Guid.Parse(user.Id),
                user.UserName!,
                user.Email!,
                roles);

            var newRefreshToken = await _refreshTokenRepository.CreateAsync(Guid.Parse(user.Id));

            return new RefreshTokenResponse(
                newToken,
                newRefreshToken,
                DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes));
        }
    }
}

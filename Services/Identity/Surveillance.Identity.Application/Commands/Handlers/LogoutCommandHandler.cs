using MediatR;
using Microsoft.Extensions.Logging;
using Surveillance.Identity.Domain.Repositories;

namespace Surveillance.Identity.Application.Commands.Handlers
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IRefreshTokenRepository refreshTokenRepository,
            ILogger<LogoutCommandHandler> logger)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _logger = logger;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Revoke all refresh tokens for the user
                await _refreshTokenRepository.RevokeAllForUserAsync(request.UserId);

                _logger.LogInformation("User {UserId} logged out successfully", request.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {UserId}", request.UserId);
                throw;
            }
        }
    }
}

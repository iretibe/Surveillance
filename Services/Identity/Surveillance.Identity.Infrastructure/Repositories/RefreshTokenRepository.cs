using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Surveillance.Identity.Domain.Entities;
using Surveillance.Identity.Domain.Repositories;
using Surveillance.Identity.Infrastructure.Data;
using System.Security.Cryptography;

namespace Surveillance.Identity.Infrastructure.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UserDbContext _context;
        private readonly ILogger<RefreshTokenRepository> _logger;

        public RefreshTokenRepository(UserDbContext context, ILogger<RefreshTokenRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateAsync(Guid userId)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = GenerateRefreshToken(),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7), // Refresh token valid for 7 days
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            await _context.Set<RefreshToken>().AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created refresh token for user {UserId}", userId);

            return refreshToken.Token;
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.Set<RefreshToken>()
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token);
        }

        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var refreshTokens = await _context.Set<RefreshToken>()
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();

            foreach (var token in refreshTokens)
            {
                token.IsRevoked = true;
                token.RevokedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Revoked all refresh tokens for user {UserId}", userId);
        }

        public async Task RevokeAsync(Guid tokenId)
        {
            var refreshToken = await _context.Set<RefreshToken>().FindAsync(tokenId);
            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                refreshToken.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Revoked refresh token {TokenId} for user {UserId}",
                    tokenId, refreshToken.UserId);
            }
        }
    }
}

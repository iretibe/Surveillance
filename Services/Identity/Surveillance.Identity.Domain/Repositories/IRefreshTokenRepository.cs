using Surveillance.Identity.Domain.Entities;

namespace Surveillance.Identity.Domain.Repositories
{
    public interface IRefreshTokenRepository
    {
        Task<string> CreateAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task RevokeAsync(Guid tokenId);
        Task RevokeAllForUserAsync(Guid userId);
    }
}

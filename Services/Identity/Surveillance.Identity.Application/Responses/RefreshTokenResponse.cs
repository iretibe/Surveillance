namespace Surveillance.Identity.Application.Responses
{
    public record RefreshTokenResponse(string Token, string RefreshToken, DateTime ExpiresAt);
}

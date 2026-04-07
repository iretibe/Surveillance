namespace Surveillance.Identity.Application.Responses
{
    public record LoginResponse(string Token, 
        string RefreshToken, DateTime ExpiresAt, 
        UserInfo User);

    public record UserInfo(Guid Id, string Username, 
        string Email, string FirstName, string LastName, 
        IEnumerable<string> Roles);
}

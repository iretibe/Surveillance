using Microsoft.AspNetCore.Identity;

namespace Surveillance.Identity.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{FirstName} {LastName}".Trim();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

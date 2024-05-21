using Microsoft.AspNetCore.Identity;

namespace BubtCommunity.Persistence.IdentityMembers;

public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}
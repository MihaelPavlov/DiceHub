using Microsoft.AspNetCore.Identity;

namespace DH.Adapter.Authentication.Entities;

public class ApplicationUser : IdentityUser
{
    public string? RefreshToken { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
}

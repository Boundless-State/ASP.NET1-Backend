using Microsoft.AspNetCore.Identity;

namespace Data.Entities;
public class UserEntity : IdentityUser
{
    public ClientEntity? Client { get; set; }
    public string? ClientId { get; set; }
    public UserProfileEntity? Profile { get; set; }
    public UserAddressEntity? Address { get; set; }
}

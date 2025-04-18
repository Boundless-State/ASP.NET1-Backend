using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class UserModel
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public UserProfileModel? Profile { get; set; }
    public UserAddressModel? Address { get; set; }
}

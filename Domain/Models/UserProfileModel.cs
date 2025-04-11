namespace Domain.Models;

public class UserProfileModel
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public string? Image { get; set; }
}

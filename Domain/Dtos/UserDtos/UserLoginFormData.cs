using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.UserDtos;

public class UserLoginFormData
{
    [Required]
    [SwaggerSchema(Description = "Användarens e-postadress som används för inloggning.")]
    public string Email { get; set; } = null!;

    [Required]
    [SwaggerSchema(Description = "Lösenord kopplat till användarens konto.")]
    public string Password { get; set; } = null!;

    [SwaggerSchema(Description = "Keep me logged in.")]
    public bool RememberMe { get; set; }
}

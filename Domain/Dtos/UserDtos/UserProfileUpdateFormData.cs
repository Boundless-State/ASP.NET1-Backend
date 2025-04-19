using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Dtos.UserDtos;

public class UserProfileUpdateFormData
{
    [SwaggerSchema(Description = "Förnamn att uppdatera.")]
    public string? FirstName { get; set; }

    [SwaggerSchema(Description = "Efternamn att uppdatera.")]
    public string? LastName { get; set; }

    [SwaggerSchema(Description = "Telefonnummer att uppdatera.")]
    public string? PhoneNumber { get; set; }

    [SwaggerSchema(Description = "Profilbild.")]
    public string? Image { get; set; }
}
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Domain.Dtos.UserDtos;

public class UserRegistrationFormData
{
    [Required]
    [SwaggerSchema(Description = "Förnamn på användaren.")]
    public string FirstName { get; set; } = null!;

    [Required]
    [SwaggerSchema(Description = "Efternamn på användaren.")]
    public string LastName { get; set; } = null!;

    [Required]
    [EmailAddress]
    [SwaggerSchema(Description = "E-postadress som används som användarnamn.")]
    public string Email { get; set; } = null!;

    [Required]
    [SwaggerSchema(Description = "Användarens lösenord.")]
    public string Password { get; set; } = null!;

    [SwaggerSchema(Description = "Telefonnummer.")]
    public string? PhoneNumber { get; set; }

    [SwaggerSchema(Description = "Gatunamn där användaren bor.")]
    public string StreetName { get; set; } = null!;

    [SwaggerSchema(Description = "Gatunummer.")]
    public string StreetNumber { get; set; } = null!;

    [SwaggerSchema(Description = "Postnummer.")]
    public string PostalCode { get; set; } = null!;

    [SwaggerSchema(Description = "Stad.")]
    public string City { get; set; } = null!;

    [SwaggerSchema(Description = "Land.")]
    public string Country { get; set; } = null!;
}

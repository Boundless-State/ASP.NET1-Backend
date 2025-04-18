namespace Domain.Models;

public class UserAddressModel
{
    public string StreetName { get; set; } = null!;
    public string? StreetNumber { get; set; }
    public PostalCodeModel PostalCode { get; set; } = null!;
}

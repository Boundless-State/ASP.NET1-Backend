using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class UserAddressEntity
{
    [Key, ForeignKey(nameof(User))]
    public string UserId { get; set; } = null!;
    public UserEntity User { get; set; } = null!;

    public string StreetName { get; set; } = null!;
    public string? StreetNumber { get; set; }

    [ForeignKey(nameof(PostalCode))]
    public string PostalCodeId { get; set; } = null!;
    public PostalCodeEntity PostalCode { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApi.Entities;

public class ProjectEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }

    [Column(TypeName = "date")]
    public DateTime StartDate { get; set; }


    [Column(TypeName = "date")]
    public DateTime? EndDate { get; set; }

    [Column(TypeName = "money")]
    public decimal? Budget { get; set; }


    [ForeignKey(nameof(Client))]
    public string ClientId { get; set; } = null!;
    public ClientEntity Client { get; set; } = null!;


    [ForeignKey(nameof(Status))]
    public int StatusId { get; set; }
    public StatusEntity Status { get; set; } = null!;


    [ForeignKey(nameof(User))]
    public string UserId { get; set; }
    public UserEntity User { get; set; } = null!;
}

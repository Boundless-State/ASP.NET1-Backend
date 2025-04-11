using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

public class ClientEntity
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ClientName { get; set; } = null!;
    public ICollection<ProjectEntity> Projects { get; set; } = [];
}

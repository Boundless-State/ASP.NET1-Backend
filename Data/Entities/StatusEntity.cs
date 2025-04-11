using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

[Index(nameof(StatusName), IsUnique = true)]
public class StatusEntity
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string StatusName { get; set; } = null!;
    public ICollection<ProjectEntity> Projects { get; set; } = [];
}

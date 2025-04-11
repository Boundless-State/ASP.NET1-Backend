using System.Collections.Generic;

namespace Domain.Models;

public class StatusModel
{
    public int Id { get; set; }
    public string StatusName { get; set; } = null!;

    public ICollection<ProjectModel>? Projects { get; set; }
}

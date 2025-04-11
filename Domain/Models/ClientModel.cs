using System.Collections.Generic;

namespace Domain.Models;

public class ClientModel
{
    public string Id { get; set; } = null!;
    public string ClientName { get; set; } = null!;

    public ICollection<ProjectModel>? Projects { get; set; }
}

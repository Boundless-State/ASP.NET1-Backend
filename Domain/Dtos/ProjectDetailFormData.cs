using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos;

public class ProjectDetailFormData
{
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }
    public ClientFormData Client { get; set; } = null!;
    public StatusFormData Status { get; set; } = null!;
    public UserFormData AssignedUser { get; set; } = null!;
}

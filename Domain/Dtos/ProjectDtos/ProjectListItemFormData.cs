using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos.ProjectDtos;

public class ProjectListItemFormData
{
    public string Id { get; set; } = null!;
    public string ProjectName { get; set; } = null!;
    public string? Image { get; set; }
    public string ClientName { get; set; } = null!;
    public string StatusName { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
}

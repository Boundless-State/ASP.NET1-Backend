using System;

namespace Domain.Models;

public class ProjectModel
{
    public string Id { get; set; } = null!;
    public string? Image { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public decimal? Budget { get; set; }


    public string ClientId { get; set; } = null!;
    public int StatusId { get; set; }
    public string UserId { get; set; } = null!;


    public ClientModel? Client { get; set; }
    public StatusModel? Status { get; set; }
    public UserModel? User { get; set; }
}

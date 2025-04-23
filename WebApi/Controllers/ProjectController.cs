using Data.Repositories;
using Domain.Dtos;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Data.Entities;
using WebApi.Entities;
using Domain.Dtos.ProjectDtos;
using Domain.Dtos.UserDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }
    //Här i de flesta metoder så la jag till Unknown i ClientName och StatusName för att undvika null referens fel,
    //då jag hade problem med det i frontend en stund.
    [HttpGet]
    public async Task<IActionResult> GetAll(bool orderByDescending = false, string? sortBy = null)
    {
        var result = await _projectService.GetAllAsync(orderByDescending, sortBy);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        var projects = result.Result!.Select(p => new ProjectListItemFormData
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Image = p.Image,
            ClientName = p.Client?.ClientName ?? "Unknown",
            StatusName = p.Status?.StatusName ?? "Unknown",
            StartDate = p.StartDate,
            EndDate = p.EndDate
        });

        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _projectService.GetByIdAsync(id);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        var project = result.Result!;

        var detailData = new ProjectDetailFormData
        {
            Id = project.Id,
            ProjectName = project.ProjectName,
            Description = project.Description,
            Image = project.Image,
            StartDate = project.StartDate,
            EndDate = project.EndDate,
            Budget = project.Budget,
            Client = new ClientFormData { ClientName = project.Client?.ClientName ?? "Unknown" },
            Status = new StatusFormData { Id = project.Status?.Id ?? 0, StatusName = project.Status?.StatusName ?? "Unknown" },
            AssignedUser = new UserFormData
            {
                Id = project.User?.Id ?? "Unknown",
                Email = project.User?.Email ?? "Unknown",
                FullName = $"{project.User?.Profile?.FirstName ?? ""} {project.User?.Profile?.LastName ?? ""}".Trim(),
                Image = project.User?.Profile?.Image
            }
        };

        return Ok(detailData);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ProjectCreateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = new ProjectEntity
        {
            Id = Guid.NewGuid().ToString(),
            ProjectName = formData.ProjectName,
            Description = formData.Description,
            Image = formData.Image,
            StartDate = formData.StartDate,
            EndDate = formData.EndDate,
            Budget = formData.Budget,
            ClientId = formData.ClientId,
            StatusId = formData.StatusId,
            UserId = formData.UserId
        };

        var result = await _projectService.CreateAsync(entity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new { id = entity.Id });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] ProjectUpdateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var entity = await _projectService.GetEntityByIdAsync(id);
        if (entity == null)
            return NotFound(new { error = "Projektet hittades inte." });

        entity.ProjectName = formData.ProjectName ?? entity.ProjectName;
        entity.Description = formData.Description ?? entity.Description;
        entity.Image = formData.Image ?? entity.Image;
        entity.StartDate = formData.StartDate ?? entity.StartDate;
        entity.EndDate = formData.EndDate ?? entity.EndDate;
        entity.Budget = formData.Budget ?? entity.Budget;
        entity.StatusId = formData.StatusId ?? entity.StatusId;

        var result = await _projectService.UpdateAsync(entity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var result = await _projectService.DeleteByIdAsync(id);

            if (!result.Succeeded)
                return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

            return NoContent();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

using Data.Repositories;
using Domain.Dtos;
using Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Data.Entities;
using WebApi.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProjectController : ControllerBase
{
    private readonly ProjectService _projectService;

    public ProjectController(ProjectService projectService)
    {
        _projectService = projectService;
    }

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
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ProjectCreateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var projectEntity = new ProjectEntity
        {
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

        var result = await _projectService.CreateAsync(projectEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = projectEntity.Id }, new { id = projectEntity.Id });
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(string id, [FromBody] ProjectUpdateFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getResult = await _projectService.GetByIdAsync(id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var project = getResult.Result!;
        var projectEntity = new ProjectEntity
        {
            Id = id,
            ProjectName = formData.ProjectName ?? project.ProjectName,
            Description = formData.Description ?? project.Description,
            Image = formData.Image ?? project.Image,
            StartDate = formData.StartDate ?? project.StartDate,
            EndDate = formData.EndDate ?? project.EndDate,
            Budget = formData.Budget ?? project.Budget,
            StatusId = formData.StatusId ?? project.StatusId,
            ClientId = project.ClientId,
            UserId = project.UserId
        };

        var result = await _projectService.UpdateAsync(projectEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(string id)
    {
        var getResult = await _projectService.GetByIdAsync(id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var project = getResult.Result!;
        var projectEntity = new ProjectEntity { Id = id };

        var result = await _projectService.DeleteAsync(projectEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }
}
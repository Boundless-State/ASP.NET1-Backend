using Data.Repositories;
using Domain.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Dtos.ProjectDtos;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ClientController : ControllerBase
{
    private readonly ClientRepository _clientRepository;
    private readonly ProjectRepository _projectRepository;

    public ClientController(ClientRepository clientRepository, ProjectRepository projectRepository)
    {
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
    }

    [HttpGet]
 
    public async Task<IActionResult> GetAll()
    {
        var result = await _clientRepository.GetAllAsync();

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return Ok(result.Result);
    }

    [HttpGet("{id}")]
    
    public async Task<IActionResult> GetById(string id)
    {
        var result = await _clientRepository.GetAsync(c => c.Id == id);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return Ok(result.Result);
    }

    [HttpGet("{id}/projects")]
   
    public async Task<IActionResult> GetClientProjects(string id)
    {
        var clientResult = await _clientRepository.ExistsAsync(c => c.Id == id);

        if (!clientResult.Succeeded)
            return StatusCode(clientResult.StatusCode ?? 500, new { error = clientResult.Error });

        var projectsResult = await _projectRepository.GetAllAsync(
            findByExpression: p => p.ClientId == id
        );

        if (!projectsResult.Succeeded)
            return StatusCode(projectsResult.StatusCode ?? 500, new { error = projectsResult.Error });

        var projects = projectsResult.Result!.Select(p => new ProjectListItemFormData
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] ClientFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var clientEntity = new ClientEntity
        {
            ClientName = formData.ClientName
        };

        var result = await _clientRepository.AddAsync(clientEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = clientEntity.Id }, new { id = clientEntity.Id });
    }

    [HttpPut("{id}")]
    [Authorize]

    public async Task<IActionResult> Update(string id, [FromBody] ClientFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getResult = await _clientRepository.GetAsync(c => c.Id == id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var clientEntity = new ClientEntity
        {
            Id = id,
            ClientName = formData.ClientName
        };

        var result = await _clientRepository.UpdateAsync(clientEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]

    public async Task<IActionResult> Delete(string id)
    {
        var getResult = await _clientRepository.GetAsync(c => c.Id == id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var clientEntity = new ClientEntity { Id = id };

        var result = await _clientRepository.DeleteAsync(clientEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }
}
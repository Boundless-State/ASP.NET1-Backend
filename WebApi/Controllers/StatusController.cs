using Data.Repositories;
using Domain.Dtos;
using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StatusController : ControllerBase
{
    private readonly IBaseRepository<StatusEntity, StatusModel> _statusRepository;

    public StatusController(IBaseRepository<StatusEntity, StatusModel> statusRepository)
    {
        _statusRepository = statusRepository;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await _statusRepository.GetAllAsync();

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return Ok(result.Result);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _statusRepository.GetAsync(s => s.Id == id);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return Ok(result.Result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] StatusFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var statusEntity = new StatusEntity
        {
            StatusName = formData.StatusName
        };

        var result = await _statusRepository.AddAsync(statusEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = statusEntity.Id }, new { id = statusEntity.Id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] StatusFormData formData)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var getResult = await _statusRepository.GetAsync(s => s.Id == id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var statusEntity = new StatusEntity
        {
            Id = id,
            StatusName = formData.StatusName
        };

        var result = await _statusRepository.UpdateAsync(statusEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var getResult = await _statusRepository.GetAsync(s => s.Id == id);

        if (!getResult.Succeeded)
            return StatusCode(getResult.StatusCode ?? 500, new { error = getResult.Error });

        var statusEntity = new StatusEntity { Id = id };

        var result = await _statusRepository.DeleteAsync(statusEntity);

        if (!result.Succeeded)
            return StatusCode(result.StatusCode ?? 500, new { error = result.Error });

        return NoContent();
    }
}
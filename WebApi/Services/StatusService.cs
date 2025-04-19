using Data.Repositories;
using Domain.Models;
using WebApi.Entities;

namespace Domain.Services;

public class StatusService
{
    private readonly StatusRepository _statusRepository;

    public StatusService(StatusRepository statusRepository)
    {
        _statusRepository = statusRepository;
    }

    public async Task<RepositoryResult<IEnumerable<StatusModel>>> GetAllAsync()
    {
        return await _statusRepository.GetAllAsync();
    }

    public async Task<RepositoryResult<StatusModel>> GetByIdAsync(int id)
    {
        return await _statusRepository.GetAsync(s => s.Id == id);
    }

    public async Task<RepositoryResult> CreateAsync(StatusEntity status)
    {
        return await _statusRepository.AddAsync(status);
    }

    public async Task<RepositoryResult> UpdateAsync(StatusEntity status)
    {
        return await _statusRepository.UpdateAsync(status);
    }

    public async Task<RepositoryResult> DeleteAsync(StatusEntity status)
    {
        return await _statusRepository.DeleteAsync(status);
    }
}


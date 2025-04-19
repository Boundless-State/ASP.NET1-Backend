using Data.Repositories;
using Domain.Dtos.ProjectDtos;
using Domain.Models;
using System.Linq;

namespace Domain.Services;

public class ClientService
{
    private readonly ClientRepository _clientRepository;
    private readonly ProjectRepository _projectRepository;

    public ClientService(ClientRepository clientRepository, ProjectRepository projectRepository)
    {
        _clientRepository = clientRepository;
        _projectRepository = projectRepository;
    }

    public async Task<RepositoryResult<IEnumerable<ClientModel>>> GetAllClientsAsync()
    {
        return await _clientRepository.GetAllAsync();
    }

    public async Task<RepositoryResult<ClientModel>> GetClientByIdAsync(string id)
    {
        return await _clientRepository.GetAsync(c => c.Id == id);
    }

    public async Task<RepositoryResult<IEnumerable<ProjectListItemFormData>>> GetClientProjectsAsync(string clientId)
    {
        var clientExists = await _clientRepository.ExistsAsync(c => c.Id == clientId);
        if (!clientExists.Succeeded)
            return new RepositoryResult<IEnumerable<ProjectListItemFormData>>
            {
                Succeeded = false,
                StatusCode = clientExists.StatusCode,
                Error = clientExists.Error
            };

        var projectsResult = await _projectRepository.GetAllAsync(
            findByExpression: p => p.ClientId == clientId
        );

        if (!projectsResult.Succeeded)
            return new RepositoryResult<IEnumerable<ProjectListItemFormData>>
            {
                Succeeded = false,
                StatusCode = projectsResult.StatusCode,
                Error = projectsResult.Error
            };

        var list = projectsResult.Result!.Select(p => new ProjectListItemFormData
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Image = p.Image,
            ClientName = p.Client?.ClientName ?? "",
            StatusName = p.Status?.StatusName ?? "",
            StartDate = p.StartDate,
            EndDate = p.EndDate
        });

        return new RepositoryResult<IEnumerable<ProjectListItemFormData>>
        {
            Succeeded = true,
            Result = list
        };
    }
}

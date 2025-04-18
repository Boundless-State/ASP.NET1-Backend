using Domain.Models;
using System.Linq.Expressions;
using Data.Repositories;
using Data.Entities;
using WebApi.Entities;


namespace Domain.Services
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<RepositoryResult<IEnumerable<ProjectModel>>> GetAllAsync(bool orderByDescending = false, string? sortBy = null)
        {
            Expression<Func<ProjectEntity, object>>? sortByExpression = sortBy?.ToLower() switch
            {
                "name" => project => project.ProjectName,
                "start" => project => project.StartDate,
                "end" => project => project.EndDate ?? DateTime.MaxValue,
                "budget" => project => project.Budget ?? 0,
                "status" => project => project.StatusId,
                _ => null
            };

            return await _projectRepository.GetAllAsync(orderByDescending, sortByExpression);
        }

        public async Task<RepositoryResult<ProjectModel>> GetByIdAsync(string id)
        {
            return await _projectRepository.GetAsync(p => p.Id == id);
        }

        public async Task<RepositoryResult> CreateAsync(ProjectEntity project)
        {
            return await _projectRepository.AddAsync(project);
        }

        public async Task<RepositoryResult> UpdateAsync(ProjectEntity project)
        {
            return await _projectRepository.UpdateAsync(project);
        }

        public async Task<RepositoryResult> DeleteAsync(ProjectEntity project)
        {
            return await _projectRepository.DeleteAsync(project);
        }
    }
}


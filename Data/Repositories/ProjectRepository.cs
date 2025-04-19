using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using WebApi.Entities;
using Domain.Extentions;

namespace Data.Repositories;

public class ProjectRepository : BaseRepository<ProjectEntity, ProjectModel>
{
    public ProjectRepository(DataContext context, IMemoryCache cache) : base(context, cache)
    {
    }

    public override async Task<RepositoryResult<IEnumerable<ProjectModel>>> GetAllAsync(
        bool orderByDescending = false,
        Expression<Func<ProjectEntity, object>>? sortByExpression = null,
        Expression<Func<ProjectEntity, bool>>? findByExpression = null,
        int setCacheTime = 5,
        params Expression<Func<ProjectEntity, object>>[] includes)
    {
        try
        {
            var query = _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Status)
                .Include(p => p.User)
                .AsQueryable();

            if (findByExpression != null)
                query = query.Where(findByExpression);

            if (sortByExpression != null)
                query = orderByDescending
                    ? query.OrderByDescending(sortByExpression)
                    : query.OrderBy(sortByExpression);

            var entities = await query.ToListAsync();

            var models = entities.Select(entity =>
            {
                var model = entity.MapTo<ProjectModel>();
                model.Client = entity.Client?.MapTo<ClientModel>();
                model.Status = entity.Status?.MapTo<StatusModel>();
                model.User = entity.User?.MapTo<UserModel>();
                return model;
            });

            return new RepositoryResult<IEnumerable<ProjectModel>>
            {
                Succeeded = true,
                Result = models
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<IEnumerable<ProjectModel>>
            {
                Succeeded = false,
                Error = ex.Message,
                StatusCode = 500
            };
        }
    }

    public override async Task<RepositoryResult<ProjectModel>> GetAsync(
        Expression<Func<ProjectEntity, bool>> findByExpression,
        int setCacheTime = 5,
        params Expression<Func<ProjectEntity, object>>[] includes)
    {
        try
        {
            var entity = await _context.Projects
                .Include(p => p.Client)
                .Include(p => p.Status)
                .Include(p => p.User)
                .FirstOrDefaultAsync(findByExpression);

            if (entity == null)
            {
                return new RepositoryResult<ProjectModel>
                {
                    Succeeded = false,
                    Error = "Project not found",
                    StatusCode = 404
                };
            }

            var model = entity.MapTo<ProjectModel>();
            model.Client = entity.Client?.MapTo<ClientModel>();
            model.Status = entity.Status?.MapTo<StatusModel>();
            model.User = entity.User?.MapTo<UserModel>();

            return new RepositoryResult<ProjectModel>
            {
                Succeeded = true,
                Result = model
            };
        }
        catch (Exception ex)
        {
            return new RepositoryResult<ProjectModel>
            {
                Succeeded = false,
                Error = ex.Message,
                StatusCode = 500
            };
        }
    }

    public async Task<ProjectEntity?> GetEntityByIdAsync(string id)
    {
        return await _context.Projects
            .Include(p => p.Client)
            .Include(p => p.Status)
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

using Data.Contexts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;
using WebApi.Entities;

namespace Data.Repositories;
//Använde AI här för att hjälpa mig att förstå varför jag inte fick det att fungera med de avancerade begreppen.
// känner inte igen det så ville bara meddela att jag skrev av lite här i för att få ihop metodiken.
public class ProjectRepository : BaseRepository<ProjectEntity, ProjectModel>
{
    public ProjectRepository(DataContext context, IMemoryCache cache): base(context, cache)
    {
    }
    public override async Task<RepositoryResult<IEnumerable<ProjectModel>>> GetAllAsync(
                            bool orderByDescending = false,
                            Expression<Func<ProjectEntity, 
                            object>>? sortByExpression = null,
                            Expression<Func<ProjectEntity, bool>>? findByExpression = null,
                            int setCacheTime = 5,
                            params Expression<Func<ProjectEntity, object>>[] includes)
    {
        return await base.GetAllAsync(orderByDescending, sortByExpression, findByExpression, setCacheTime,
            e => e.Client, e => e.User, e => e.Status);
    }

    public override async Task<RepositoryResult<ProjectModel>> GetAsync(Expression<Func<ProjectEntity, bool>> findByExpression,
                            int setCacheTime = 5,
                            params Expression<Func<ProjectEntity, object>>[] includes)
    {
        return await base.GetAsync(findByExpression, setCacheTime,
            e => e.Client, e => e.User, e => e.Status);
    }
}

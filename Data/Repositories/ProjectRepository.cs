using Data.Contexts;

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Entities;

namespace Data.Repositories;

public class ProjectRepository(DataContext context) : BaseRepository<ProjectEntity>(context)
{
    public override async Task<IEnumerable<ProjectEntity>> GetAllAsync()
    {
        var entities = await _table
            .Include(x => x.Client)
            .Include(x => x.User)
            .Include(x => x.Status)
            .ToListAsync();

        return entities;
    }

    public override async Task<ProjectEntity?> GetAsync(Expression<Func<ProjectEntity, bool>> expression)
    {
        var entity = await _table
            .Include(x => x.Client)
            .Include(x => x.User)
            .Include(x => x.Status)
            .FirstOrDefaultAsync(expression);

        return entity;
    }
}

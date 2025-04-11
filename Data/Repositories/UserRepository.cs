using Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using WebApi.Entities;

namespace Data.Repositories;

public class UserRepository(DataContext context) : BaseRepository<UserEntity>(context)
{
    public override async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        var entities = await _table.ToListAsync();
        return entities;
    }

    public override async Task<UserEntity?> GetAsync(Expression<Func<UserEntity, bool>> expression)
    {
        var entity = await _table.FirstOrDefaultAsync(expression);
        return entity;
    }
}

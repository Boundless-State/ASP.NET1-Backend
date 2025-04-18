using Data.Contexts;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using WebApi.Entities;

namespace Data.Repositories;

public class StatusRepository : BaseRepository<StatusEntity, StatusModel>, IBaseRepository<StatusEntity, StatusModel>
{
    public StatusRepository(DataContext context, IMemoryCache cache) : base(context, cache)
    {
    }
}

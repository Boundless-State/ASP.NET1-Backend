using Data.Contexts;
using WebApi.Entities;

namespace Data.Repositories;

public class StatusRepository(DataContext context) : BaseRepository<StatusEntity>(context)
{
}

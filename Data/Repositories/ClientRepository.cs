using Data.Contexts;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public class ClientRepository : BaseRepository<ClientEntity, ClientModel>
{
    public ClientRepository(DataContext context, IMemoryCache cache): base(context, cache)
    {
    }
}

using Data.Contexts;


namespace Data.Repositories;

public class ClientRepository(DataContext context) : BaseRepository<ClientEntity>(context)
{
}

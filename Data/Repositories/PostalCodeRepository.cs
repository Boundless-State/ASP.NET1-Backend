using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;

public class PostalCodeRepository(DataContext context, IMemoryCache cache) : BaseRepository<PostalCodeEntity, PostalCodeModel>(context, cache)
{
}

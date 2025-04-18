using Data.Contexts;
using Data.Entities;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Data.Repositories;
public interface IUserRepository : IBaseRepository<UserEntity, UserModel>
{
    Task<RepositoryResult> AddAsync(UserEntity entity, string password);
}
public class UserRepository(DataContext context, IMemoryCache cache, UserManager<UserEntity> userManager) : BaseRepository<UserEntity, UserModel>(context, cache), IUserRepository
{
    private readonly UserManager<UserEntity> _userManager = userManager;
    public async Task<RepositoryResult> AddAsync(UserEntity entity, string password)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };
        try
        {
            var result = await _userManager.CreateAsync(entity, password);
            if (result.Succeeded)
            {
                ClearCache();
                return new RepositoryResult { Succeeded = true, StatusCode = 201 };
            }
            throw new Exception("Unable to create user by using usermanager.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }
}
using Data.Contexts;
using Domain.Extentions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Linq.Expressions;


namespace Data.Repositories;

public interface IBaseRepository<TEntity, TModel> where TEntity : class
{
    Task<RepositoryResult> AddAsync(TEntity entity);
    Task<RepositoryResult> DeleteAsync(TEntity entity);
    Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> findByExpression);
    Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortByExpression = null, Expression<Func<TEntity, bool>>? findByExpression = null, int setCacheTime = 5, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findByExpression, int setCacheTime = 5, params Expression<Func<TEntity, object>>[] includes);
    Task<RepositoryResult> UpdateAsync(TEntity entity);
}

public abstract class BaseRepository<TEntity, TModel> : IBaseRepository<TEntity, TModel> where TEntity : class
{
    protected readonly DataContext _context;
    protected readonly IMemoryCache _cache;
    protected readonly DbSet<TEntity> _table;
    protected readonly string _cacheKey;

    protected BaseRepository(DataContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;

        _table = _context.Set<TEntity>();
        _cacheKey = $"{typeof(TEntity).Name}_All";
    }

    public void ClearCache()
    {
        _cache.Remove(_cacheKey);
        foreach (var key in CacheManager.CacheKeys)
            _cache.Remove(key);

        CacheManager.CacheKeys.Clear();
    }



    public async Task<RepositoryResult> ExistsAsync(Expression<Func<TEntity, bool>> findByExpression)
    {
        var result = await _table.AnyAsync(findByExpression);
        return result
            ? new RepositoryResult { Succeeded = true, StatusCode = 200 }
            : new RepositoryResult { Succeeded = false, StatusCode = 404 };
    }


    public virtual async Task<RepositoryResult> AddAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _table.Add(entity);
            await _context.SaveChangesAsync();

            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 201 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult> UpdateAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _table.Update(entity);
            await _context.SaveChangesAsync();

            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult> DeleteAsync(TEntity entity)
    {
        if (entity == null)
            return new RepositoryResult { Succeeded = false, StatusCode = 400 };

        try
        {
            _table.Remove(entity);
            await _context.SaveChangesAsync();

            ClearCache();
            return new RepositoryResult { Succeeded = true, StatusCode = 200 };
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new RepositoryResult { Succeeded = false, StatusCode = 500, Error = ex.Message };
        }
    }

    public virtual async Task<RepositoryResult<TModel>> GetAsync(Expression<Func<TEntity, bool>> findByExpression, int setCacheTime = 5, params Expression<Func<TEntity, object>>[] includes)
    {

        var includeKey = includes != null && includes.Length > 0
            ? string.Join(",", includes.Select(x => x.ToString()))
            : "";

        var cacheKey = $"{typeof(TEntity).Name}_Get_{findByExpression}_{includeKey}";
        if (!CacheManager.CacheKeys.Contains(cacheKey))
            CacheManager.CacheKeys.Add(cacheKey);


        if (_cache.TryGetValue(cacheKey, out TModel? cachedModel))
        {
            return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = cachedModel };
        }


        IQueryable<TEntity> query = _table;

        if (includes != null && includes.Length > 0)
            foreach (var include in includes)
                query = query.Include(include);

        var entity = await query.FirstOrDefaultAsync(findByExpression);
        if (entity == null)
            return new RepositoryResult<TModel> { Succeeded = false, StatusCode = 404 };

        var model = entity.MapTo<TModel>();

        _cache.Set(cacheKey, model, TimeSpan.FromMinutes(setCacheTime));
        return new RepositoryResult<TModel> { Succeeded = true, StatusCode = 200, Result = model };
    }

    public virtual async Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortByExpression = null, Expression<Func<TEntity, bool>>? findByExpression = null, int setCacheTime = 5, params Expression<Func<TEntity, object>>[] includes)
    {
        var findByExpressionKey = findByExpression != null ? findByExpression.ToString() : "";
        var sortByExpressionKey = sortByExpression != null ? sortByExpression.ToString() : "";
        var includesKey = includes != null && includes.Length > 0 ? string.Join(",", includes.Select(x => x.ToString())) : "";

        var cacheKey = $"{typeof(TEntity).Name}_All_{findByExpressionKey}_{sortByExpressionKey}_{includesKey}";
        if (!CacheManager.CacheKeys.Contains(cacheKey))
            CacheManager.CacheKeys.Add(cacheKey);


        if (_cache.TryGetValue(cacheKey, out IEnumerable<TModel>? cachedModels))
        {
            return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = cachedModels };
        }

        IQueryable<TEntity> query = _table;

        if (findByExpression != null)
            query = query.Where(findByExpression);

        if (includes != null && includes.Length > 0)
            foreach (var include in includes)
                query = query.Include(include);

        if (sortByExpression != null)
            query = orderByDescending ? query.OrderByDescending(sortByExpression) : query.OrderBy(sortByExpression);

        var entities = await query.ToListAsync();
        var list = entities.Select(entity => entity.MapTo<TModel>());

        _cache.Set(cacheKey, list, TimeSpan.FromMinutes(setCacheTime));
        return new RepositoryResult<IEnumerable<TModel>> { Succeeded = true, StatusCode = 200, Result = list };
    }
}
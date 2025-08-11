using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using walletv2.Data.DataContext;
using walletv2.Data.Entities;

namespace walletv2.Data.Repositories;

public interface IBaseRepository<T> where T : IBaseEntity, new()
{
    IQueryable<T> Table { get; }
    IQueryable<T> TableNoTracking { get; }

    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();

    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
    Task<IQueryable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    Task SaveChangesAsync();

    /// <summary>
    /// join some tables given
    /// </summary>
    /// <typeparam name="TJoin"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="joinSet"></param>
    /// <param name="outerKey"></param>
    /// <param name="innerKey"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IQueryable<TResult> JoinWith<TJoin, TKey, TResult>(
        IQueryable<TJoin> joinSet,
        Expression<Func<T, TKey>> outerKey,
        Expression<Func<TJoin, TKey>> innerKey,
        Expression<Func<T, TJoin, TResult>> resultSelector
        ) where TJoin : class;

}

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : class, IBaseEntity, new()
{
    private readonly Walletv2DbContext _context;
    private readonly DbSet<TEntity> _entities;

    public BaseRepository(Walletv2DbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _entities = context.Set<TEntity>();
    }
    public IQueryable<TEntity> Table => _entities.Where(e => !e.isDeleted);
    public IQueryable<TEntity> TableNoTracking => _entities.AsNoTracking().Where(e => !e.isDeleted);

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().Where(e => !e.isDeleted).ToListAsync();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        entity.Create();
        await _context.Set<TEntity>().AddAsync(entity);
        //await SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        entity.Update();
        _context.Set<TEntity>().Update(entity);
        //await SaveChangesAsync();
        return await Task.FromResult(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
        entity.Delete();
        _context.Set<TEntity>().Update(entity);
        //await SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.Id == id && !e.isDeleted);
    }

    public async Task<IQueryable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_context.Set<TEntity>().Where(predicate).Where(e => !e.isDeleted));
    }

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().AnyAsync(predicate);
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _context.Set<TEntity>().CountAsync(predicate);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    /// <summary>
    /// join some tables given
    /// </summary>
    /// <typeparam name="TJoin"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="joinSet"></param>
    /// <param name="outerKey"></param>
    /// <param name="innerKey"></param>
    /// <param name="resultSelector"></param>
    /// <returns></returns>
    public IQueryable<TResult> JoinWith<TJoin, TKey, TResult>(IQueryable<TJoin> joinSet, 
        Expression<Func<TEntity, TKey>> outerKey, 
        Expression<Func<TJoin, TKey>> innerKey, 
        Expression<Func<TEntity, TJoin, TResult>> resultSelector) where TJoin : class
    {
        return _entities.Join(joinSet, outerKey, innerKey, resultSelector);
    }
}
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using walletv2.Data.Entities;

namespace walletv2.Data.Repositories;

public interface IBaseRepository<T> where T : IBaseEntity, new()
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();

    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);

    Task<bool> ExistsAsync(Guid id);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);

    Task SaveChangesAsync();
}

public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
    where TEntity : class, IBaseEntity, new()
    where TContext : DbContext
{
    private readonly TContext _context;
    public BaseRepository(TContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

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
        _context.Set<TEntity>().Add(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        entity.Update();
        _context.Set<TEntity>().Update(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException($"Entity with ID {id} not found.");
        entity.Delete();
        _context.Set<TEntity>().Update(entity);
        await SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Set<TEntity>().AnyAsync(e => e.id == id && !e.isDeleted);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await Task.FromResult(_context.Set<TEntity>().Where(predicate).Where(e => !e.isDeleted).ToList());
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
}
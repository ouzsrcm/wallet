using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wallet.DataLayer.Context;
using Wallet.Entities.Base.Concrete;
using Wallet.Services.RepositoryBase.Abstract;

namespace Wallet.Services.RepositoryBase.Concrete;

public class BaseRepository<T> : IBaseRepository<T> where T : SoftDeleteEntity
{
    private readonly WalletDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(WalletDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Query Methods
    public IQueryable<T> GetAll(bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return query;
    }

    public IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, bool tracking = true)
    {
        var query = _dbSet.Where(predicate);
        if (!tracking)
            query = query.AsNoTracking();
        return query;
    }

    public async Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<T?> GetByIdAsync(Guid id, bool tracking = true)
    {
        var query = _dbSet.AsQueryable();
        if (!tracking)
            query = query.AsNoTracking();
        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    // Insert Methods
    public async Task<T> AddAsync(T entity)
    {
        entity.IsDeleted = false;
        entity.CreatedDate = DateTime.UtcNow;
        entity.CreatedBy = "System";
        entity.RowVersion = 0;

        await _dbSet.AddAsync(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await SaveChangesAsync();
        return entities;
    }

    // Update Methods
    public async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await SaveChangesAsync();
        return entity;
    }

    public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await SaveChangesAsync();
        return entities;
    }

    // Delete Methods
    public async Task<bool> RemoveAsync(T entity)
    {
        _dbSet.Remove(entity);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return await SaveChangesAsync() > 0;
    }

    // Soft Delete Methods
    public async Task<bool> SoftDeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        return await SaveChangesAsync() > 0;
    }

    public async Task<bool> SoftDeleteRangeAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
        return await SaveChangesAsync() > 0;
    }

    // Save Changes
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    // Bulk Operations
    public async Task BulkInsertAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        await SaveChangesAsync();
    }

    public async Task BulkUpdateAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        await SaveChangesAsync();
    }

    public async Task BulkDeleteAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        await SaveChangesAsync();
    }

    public async Task BulkSoftDeleteAsync(IEnumerable<T> entities)
    {
        foreach (var entity in entities)
        {
            entity.IsDeleted = true;
            entity.DeletedAt = DateTime.UtcNow;
        }
        _dbSet.UpdateRange(entities);
        await SaveChangesAsync();
    }
}
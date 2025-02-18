using System.Linq.Expressions;
using Wallet.Entities.Base.Abstract;

namespace Wallet.Services.RepositoryBase.Abstract;

public interface IBaseRepository<T> where T : class, IEntity
{
    // Query Methods
    IQueryable<T> GetAll(bool tracking = true);
    IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, bool tracking = true);
    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, bool tracking = true);
    Task<T?> GetByIdAsync(Guid id, bool tracking = true);
    
    // Insert Methods
    Task<T> AddAsync(T entity);
    Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities);
    
    // Update Methods
    Task<T> UpdateAsync(T entity);
    Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities);
    
    // Delete Methods
    Task<bool> RemoveAsync(T entity);
    Task<bool> RemoveRangeAsync(IEnumerable<T> entities);
    
    // Soft Delete Methods
    Task<bool> SoftDeleteAsync(T entity);
    Task<bool> SoftDeleteRangeAsync(IEnumerable<T> entities);
    
    // Save Changes
    Task<int> SaveChangesAsync();
    
    // Bulk Operations
    Task BulkInsertAsync(IEnumerable<T> entities);
    Task BulkUpdateAsync(IEnumerable<T> entities);
    Task BulkDeleteAsync(IEnumerable<T> entities);
    Task BulkSoftDeleteAsync(IEnumerable<T> entities);
} 
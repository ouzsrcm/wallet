using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Entities.Base.Abstract;
using Wallet.Entities.Base.Concrete;

namespace Wallet.Services.UnitOfWorkBase.Abstract;

public interface IUnitOfWork : IAsyncDisposable
{
    IBaseRepository<T> GetRepository<T>() where T : SoftDeleteEntity, IEntity;
    Task<int> SaveChangesAsync();
    Task<bool> SaveEntitiesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
} 
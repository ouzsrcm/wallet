using Microsoft.EntityFrameworkCore.Storage;
using Wallet.DataLayer.Context;
using Wallet.Entities.Base.Abstract;
using Wallet.Entities.Base.Concrete;
using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Services.RepositoryBase.Concrete;
using Wallet.Services.UnitOfWorkBase.Abstract;

using Wallet.Entities.EntityObjects;

namespace Wallet.Services.UnitOfWorkBase.Concrete;

public class UnitOfWork : IUnitOfWork
{
    private readonly WalletDbContext _context;
    private readonly Dictionary<Type, object> _repositories;
    private IDbContextTransaction _currentTransaction;
    private bool _disposed;

    public UnitOfWork(WalletDbContext context)
    {
        _context = context;
        _repositories = new Dictionary<Type, object>(){
            {typeof(User), new BaseRepository<User>(_context)},
            {typeof(UserCredential), new BaseRepository<UserCredential>(_context)},
            {typeof(Person), new BaseRepository<Person>(_context)},
            {typeof(PersonAddress), new BaseRepository<PersonAddress>(_context)},
            {typeof(Nationality), new BaseRepository<Nationality>(_context)},
            {typeof(Message), new BaseRepository<Message>(_context)},
            {typeof(MessageAttachment), new BaseRepository<MessageAttachment>(_context)},
            {typeof(AuditLog), new BaseRepository<AuditLog>(_context)},
            {typeof(Transaction), new BaseRepository<Transaction>(_context)},
            {typeof(Category), new BaseRepository<Category>(_context)},
            {typeof(Receipt), new BaseRepository<Receipt>(_context)},
            {typeof(ReceiptItem), new BaseRepository<ReceiptItem>(_context)},
            {typeof(Language), new BaseRepository<Language>(_context)},
        };
    }

    public IBaseRepository<T> GetRepository<T>() where T : SoftDeleteEntity, IEntity
    {
        var type = typeof(T);

        if (!_repositories.ContainsKey(type))
        {
            var repository = new BaseRepository<T>(_context);
            _repositories.Add(type, repository);
        }

        return (IBaseRepository<T>)_repositories[type];
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<bool> SaveEntitiesAsync()
    {
        try
        {
            // Pre-save activities (if any)
            await BeforeChangeSave();
            
            // Save changes
            await _context.SaveChangesAsync();
            
            // Post-save activities (if any)
            await AfterChangeSave();
            
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task BeginTransactionAsync()
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    protected virtual async Task BeforeChangeSave()
    {
        // Implement pre-save logic here
        await Task.CompletedTask;
    }

    protected virtual async Task AfterChangeSave()
    {
        // Implement post-save logic here
        await Task.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }

                await _context.DisposeAsync();
            }

            _disposed = true;
        }
    }
} 
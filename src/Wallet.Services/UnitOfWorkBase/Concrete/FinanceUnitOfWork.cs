using Wallet.DataLayer.Context;
using Wallet.Entities.EntityObjects;
using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Services.RepositoryBase.Concrete;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.UnitOfWorkBase.Concrete;

public class FinanceUnitOfWork : IFinanceUnitOfWork
{
    private readonly WalletDbContext _context;

    public IBaseRepository<Transaction> Transactions { get; }
    public IBaseRepository<Category> Categories { get; }
    public IBaseRepository<Receipt> Receipts { get; }
    public IBaseRepository<ReceiptItem> ReceiptItems { get; }

    public FinanceUnitOfWork(WalletDbContext context)
    {
        _context = context;
        Transactions = new BaseRepository<Transaction>(_context);
        Categories = new BaseRepository<Category>(_context);
        Receipts = new BaseRepository<Receipt>(_context);
        ReceiptItems = new BaseRepository<ReceiptItem>(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
} 
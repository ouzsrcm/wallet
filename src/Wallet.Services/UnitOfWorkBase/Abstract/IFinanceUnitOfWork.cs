using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Entities.EntityObjects;

namespace Wallet.Services.UnitOfWorkBase.Abstract;

public interface IFinanceUnitOfWork
{
    IBaseRepository<Transaction> Transactions { get; }
    IBaseRepository<Category> Categories { get; }
    IBaseRepository<Receipt> Receipts { get; }
    IBaseRepository<ReceiptItem> ReceiptItems { get; }
    
    Task<int> SaveChangesAsync();
} 
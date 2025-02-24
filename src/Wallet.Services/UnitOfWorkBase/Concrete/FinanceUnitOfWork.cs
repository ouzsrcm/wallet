using Wallet.DataLayer.Context;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.UnitOfWorkBase.Concrete;

public class FinanceUnitOfWork : UnitOfWork, IFinanceUnitOfWork
{
    public FinanceUnitOfWork(WalletDbContext context) : base(context)
    {
    }
} 
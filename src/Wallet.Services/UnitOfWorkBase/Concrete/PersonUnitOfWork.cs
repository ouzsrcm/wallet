using Wallet.DataLayer.Context;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Entities.EntityObjects;

namespace Wallet.Services.UnitOfWorkBase.Concrete;

public class PersonUnitOfWork : UnitOfWork, IPersonUnitOfWork
{
    public PersonUnitOfWork(WalletDbContext context) : base(context)
    {
        
    }
} 
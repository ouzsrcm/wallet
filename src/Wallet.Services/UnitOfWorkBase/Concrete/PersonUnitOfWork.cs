using Wallet.DataLayer.Context;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Entities.EntityObjects;

namespace Wallet.Services.UnitOfWorkBase.Concrete;

public class PersonUnitOfWork : UnitOfWork, IPersonUnitOfWork
{
    // Person ve ilişkili repository'ler
    private readonly IBaseRepository<Person> _persons;
    private readonly IBaseRepository<PersonAddress> _personAddresses;
    private readonly IBaseRepository<PersonContact> _personContacts;
    private readonly IBaseRepository<Nationality> _nationalities;

    // User ve ilişkili repository'ler
    private readonly IBaseRepository<User> _users;
    private readonly IBaseRepository<UserCredential> _userCredentials;

    public PersonUnitOfWork(WalletDbContext context) : base(context)
    {
        // Repository'leri initialize et
        _persons = GetRepository<Person>();
        _personAddresses = GetRepository<PersonAddress>();
        _personContacts = GetRepository<PersonContact>();
        _nationalities = GetRepository<Nationality>();
        
        _users = GetRepository<User>();
        _userCredentials = GetRepository<UserCredential>();
    }

    // Person ve ilişkili property'ler
    public IBaseRepository<Person> Persons => _persons;
    public IBaseRepository<PersonAddress> PersonAddresses => _personAddresses;
    public IBaseRepository<PersonContact> PersonContacts => _personContacts;
    public IBaseRepository<Nationality> Nationalities => _nationalities;

    // User ve ilişkili property'ler
    public IBaseRepository<User> Users => _users;
    public IBaseRepository<UserCredential> UserCredentials => _userCredentials;
} 
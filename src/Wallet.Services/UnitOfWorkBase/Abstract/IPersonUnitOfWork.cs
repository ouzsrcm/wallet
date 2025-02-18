using Wallet.Services.RepositoryBase.Abstract;
using Wallet.Entities.EntityObjects;

namespace Wallet.Services.UnitOfWorkBase.Abstract;

public interface IPersonUnitOfWork : IUnitOfWork
{
    // Person ve ilişkili entity'ler
    IBaseRepository<Person> Persons { get; }
    IBaseRepository<PersonAddress> PersonAddresses { get; }
    IBaseRepository<PersonContact> PersonContacts { get; }
    IBaseRepository<Nationality> Nationalities { get; }

    // User ve ilişkili entity'ler
    IBaseRepository<User> Users { get; }
    IBaseRepository<UserCredential> UserCredentials { get; }

    // Mesajlar
    IBaseRepository<Message> Messages { get; }
    IBaseRepository<MessageAttachment> MessageAttachments { get; }
} 
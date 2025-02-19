using Wallet.Services.DTOs.Person;

namespace Wallet.Services.Abstract;

public interface IPersonService
{
    // Person CRUD
    Task<PersonDto> GetPersonByIdAsync(Guid id);
    Task<PersonDto> CreatePersonAsync(PersonDto personDto);
    Task<PersonDto> UpdatePersonAsync(Guid id, PersonDto personDto);
    Task DeletePersonAsync(Guid id);
    
    // Address CRUD
    Task<PersonAddressDto> AddAddressAsync(Guid personId, PersonAddressDto addressDto);
    Task<PersonAddressDto> UpdateAddressAsync(Guid addressId, PersonAddressDto addressDto);
    Task DeleteAddressAsync(Guid addressId);
    Task<PersonAddressDto> SetDefaultAddressAsync(Guid addressId);
    
    // Contact CRUD
    Task<PersonContactDto> AddContactAsync(Guid personId, PersonContactDto contactDto);
    Task<PersonContactDto> UpdateContactAsync(Guid contactId, PersonContactDto contactDto);
    Task DeleteContactAsync(Guid contactId);
    Task<PersonContactDto> SetDefaultContactAsync(Guid contactId);
} 
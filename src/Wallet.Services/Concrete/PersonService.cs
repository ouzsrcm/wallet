using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Person;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.Concrete;

public class PersonService : IPersonService
{
    private readonly IPersonUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public PersonService(IPersonUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PersonDto> GetPersonByIdAsync(Guid id)
    {
        var person = await _unitOfWork.Persons
            .GetWhere(p => p.Id == id && !p.IsDeleted)
            .Select(p => new PersonDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Language = p.Language,
                TimeZone = p.TimeZone,
                Currency = p.Currency,
                Addresses = p.Addresses
                    .Where(a => !a.IsDeleted)
                    .Select(a => new PersonAddressDto
                    {
                        Id = a.Id,
                        AddressType = a.AddressType,
                        AddressName = a.AddressName,
                        AddressLine1 = a.AddressLine1,
                        AddressLine2 = a.AddressLine2,
                        District = a.District,
                        City = a.City,
                        State = a.State,
                        Country = a.Country,
                        PostalCode = a.PostalCode,
                        IsDefault = a.IsDefault
                    }).ToList(),
                Contacts = p.Contacts
                    .Where(c => !c.IsDeleted)
                    .Select(c => new PersonContactDto
                    {
                        Id = c.Id,
                        ContactType = c.ContactType,
                        ContactName = c.ContactName,
                        ContactValue = c.ContactValue,
                        CountryCode = c.CountryCode,
                        AreaCode = c.AreaCode,
                        IsDefault = c.IsDefault,
                        IsPrimary = c.IsPrimary
                    }).ToList()
            })
            .FirstOrDefaultAsync() 
            ?? throw new Exception("Person not found");

        return person;
    }

    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        var person = _mapper.Map<Person>(personDto);
        await _unitOfWork.Persons.AddAsync(person);
        return _mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDto> UpdatePersonAsync(Guid id, PersonDto personDto)
    {
        var person = await _unitOfWork.Persons.GetByIdAsync(id)
            ?? throw new Exception("Person not found");

        _mapper.Map(personDto, person);
        await _unitOfWork.Persons.UpdateAsync(person);
        return _mapper.Map<PersonDto>(person);
    }

    public async Task DeletePersonAsync(Guid id)
    {
        var person = await _unitOfWork.Persons.GetByIdAsync(id)
            ?? throw new Exception("Person not found");

        await _unitOfWork.Persons.SoftDeleteAsync(person);
    }

    // Address operations
    public async Task<PersonAddressDto> AddAddressAsync(Guid personId, PersonAddressDto addressDto)
    {
        var person = await _unitOfWork.Persons.GetByIdAsync(personId)
            ?? throw new Exception("Person not found");

        var address = _mapper.Map<PersonAddress>(addressDto);
        address.PersonId = personId;

        if (address.IsDefault)
        {
            // Diğer varsayılan adresleri güncelle
            var defaultAddresses = await _unitOfWork.PersonAddresses
                .GetWhere(a => a.PersonId == personId && a.IsDefault)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
                await _unitOfWork.PersonAddresses.UpdateAsync(defaultAddress);
            }
        }

        await _unitOfWork.PersonAddresses.AddAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    public async Task<PersonAddressDto> UpdateAddressAsync(Guid addressId, PersonAddressDto addressDto)
    {
        var address = await _unitOfWork.PersonAddresses.GetByIdAsync(addressId)
            ?? throw new Exception("Address not found");

        if (addressDto.IsDefault && !address.IsDefault)
        {
            // Diğer varsayılan adresleri güncelle
            var defaultAddresses = await _unitOfWork.PersonAddresses
                .GetWhere(a => a.PersonId == address.PersonId && a.IsDefault)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
                await _unitOfWork.PersonAddresses.UpdateAsync(defaultAddress);
            }
        }

        _mapper.Map(addressDto, address);
        await _unitOfWork.PersonAddresses.UpdateAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    public async Task DeleteAddressAsync(Guid addressId)
    {
        var address = await _unitOfWork.PersonAddresses.GetByIdAsync(addressId)
            ?? throw new Exception("Address not found");

        await _unitOfWork.PersonAddresses.SoftDeleteAsync(address);
    }

    public async Task<PersonAddressDto> SetDefaultAddressAsync(Guid addressId)
    {
        var address = await _unitOfWork.PersonAddresses.GetByIdAsync(addressId)
            ?? throw new Exception("Address not found");

        // Diğer varsayılan adresleri güncelle
        var defaultAddresses = await _unitOfWork.PersonAddresses
            .GetWhere(a => a.PersonId == address.PersonId && a.IsDefault)
            .ToListAsync();

        foreach (var defaultAddress in defaultAddresses)
        {
            defaultAddress.IsDefault = false;
            await _unitOfWork.PersonAddresses.UpdateAsync(defaultAddress);
        }

        address.IsDefault = true;
        await _unitOfWork.PersonAddresses.UpdateAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    // Contact operations
    public async Task<PersonContactDto> AddContactAsync(Guid personId, PersonContactDto contactDto)
    {
        var person = await _unitOfWork.Persons.GetByIdAsync(personId)
            ?? throw new Exception("Person not found");

        var contact = _mapper.Map<PersonContact>(contactDto);
        contact.PersonId = personId;

        if (contact.IsDefault || contact.IsPrimary)
        {
            // Diğer varsayılan/birincil kişileri güncelle
            var existingContacts = await _unitOfWork.PersonContacts
                .GetWhere(c => c.PersonId == personId && 
                              (c.IsDefault || c.IsPrimary) && 
                              c.ContactType == contact.ContactType)
                .ToListAsync();

            foreach (var existingContact in existingContacts)
            {
                if (contact.IsDefault) existingContact.IsDefault = false;
                if (contact.IsPrimary) existingContact.IsPrimary = false;
                await _unitOfWork.PersonContacts.UpdateAsync(existingContact);
            }
        }

        await _unitOfWork.PersonContacts.AddAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }

    public async Task<PersonContactDto> UpdateContactAsync(Guid contactId, PersonContactDto contactDto)
    {
        var contact = await _unitOfWork.PersonContacts.GetByIdAsync(contactId)
            ?? throw new Exception("Contact not found");

        if ((contactDto.IsDefault && !contact.IsDefault) || 
            (contactDto.IsPrimary && !contact.IsPrimary))
        {
            // Diğer varsayılan/birincil kişileri güncelle
            var existingContacts = await _unitOfWork.PersonContacts
                .GetWhere(c => c.PersonId == contact.PersonId && 
                              (c.IsDefault || c.IsPrimary) && 
                              c.ContactType == contact.ContactType)
                .ToListAsync();

            foreach (var existingContact in existingContacts)
            {
                if (contactDto.IsDefault) existingContact.IsDefault = false;
                if (contactDto.IsPrimary) existingContact.IsPrimary = false;
                await _unitOfWork.PersonContacts.UpdateAsync(existingContact);
            }
        }

        _mapper.Map(contactDto, contact);
        await _unitOfWork.PersonContacts.UpdateAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }

    public async Task DeleteContactAsync(Guid contactId)
    {
        var contact = await _unitOfWork.PersonContacts.GetByIdAsync(contactId)
            ?? throw new Exception("Contact not found");

        await _unitOfWork.PersonContacts.SoftDeleteAsync(contact);
    }

    public async Task<PersonContactDto> SetDefaultContactAsync(Guid contactId)
    {
        var contact = await _unitOfWork.PersonContacts.GetByIdAsync(contactId)
            ?? throw new Exception("Contact not found");

        // Diğer varsayılan kişileri güncelle
        var defaultContacts = await _unitOfWork.PersonContacts
            .GetWhere(c => c.PersonId == contact.PersonId && 
                          c.IsDefault && 
                          c.ContactType == contact.ContactType)
            .ToListAsync();

        foreach (var defaultContact in defaultContacts)
        {
            defaultContact.IsDefault = false;
            await _unitOfWork.PersonContacts.UpdateAsync(defaultContact);
        }

        contact.IsDefault = true;
        await _unitOfWork.PersonContacts.UpdateAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }
} 
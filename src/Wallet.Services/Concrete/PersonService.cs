using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Person;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Services.Exceptions;

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
        var person = await _unitOfWork.GetRepository<Person>()
            .GetWhere(p => p.Id == id)
            .Select(p => new PersonDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                MiddleName = p.MiddleName,
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                NationalityId = p.NationalityId,
                TaxNumber = p.TaxNumber,
                IdNumber = p.IdNumber
            })
            .FirstOrDefaultAsync();

        if (person == null)
            throw new NotFoundException("Person not found");

        return person;
    }

    public async Task<PersonDto> CreatePersonAsync(PersonDto personDto)
    {
        var person = _mapper.Map<Person>(personDto);
        await _unitOfWork.GetRepository<Person>().AddAsync(person);
        return _mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDto> UpdatePersonAsync(Guid id, PersonDto personDto)
    {
        var person = await _unitOfWork.GetRepository<Person>().GetByIdAsync(id)
            ?? throw new Exception("Person not found");

        _mapper.Map(personDto, person);
        await _unitOfWork.GetRepository<Person>().UpdateAsync(person);
        return _mapper.Map<PersonDto>(person);
    }

    public async Task DeletePersonAsync(Guid id)
    {
        var person = await _unitOfWork.GetRepository<Person>().GetByIdAsync(id)
            ?? throw new Exception("Person not found");

        await _unitOfWork.GetRepository<Person>().SoftDeleteAsync(person);
    }

    // Address operations
    public async Task<PersonAddressDto> AddAddressAsync(Guid personId, PersonAddressDto addressDto)
    {
        var person = await _unitOfWork.GetRepository<Person>().GetByIdAsync(personId)
            ?? throw new Exception("Person not found");

        var address = _mapper.Map<PersonAddress>(addressDto);
        address.PersonId = personId;
        address.IsDefault = false;
        address.IsVerified = false;
        address.RowVersion = 0;
        address.CreatedDate = DateTime.UtcNow;
        address.CreatedBy = "System";
        address.IsDeleted = false;

        if (address.IsDefault)
        {
            // Diğer varsayılan adresleri güncelle
            var defaultAddresses = await _unitOfWork.GetRepository<PersonAddress>()
                .GetWhere(a => a.PersonId == personId && a.IsDefault)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
                await _unitOfWork.GetRepository<PersonAddress>().UpdateAsync(defaultAddress);
            }
        }

        await _unitOfWork.GetRepository<PersonAddress>().AddAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    public async Task<PersonAddressDto> UpdateAddressAsync(Guid addressId, PersonAddressDto addressDto)
    {
        var address = await _unitOfWork.GetRepository<PersonAddress>().GetByIdAsync(addressId)
            ?? throw new NotFoundException("Address not found");

        // ID'yi güncelleme, sadece diğer özellikleri güncelle
        address.AddressType = addressDto.AddressType;
        address.AddressName = addressDto.AddressName;
        address.AddressLine1 = addressDto.AddressLine1;
        address.AddressLine2 = addressDto.AddressLine2;
        address.District = addressDto.District;
        address.City = addressDto.City;
        address.State = addressDto.State;
        address.Country = addressDto.Country;
        address.PostalCode = addressDto.PostalCode;
        address.IsDefault = addressDto.IsDefault;

        await _unitOfWork.GetRepository<PersonAddress>().UpdateAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    public async Task DeleteAddressAsync(Guid addressId)
    {
        var address = await _unitOfWork.GetRepository<PersonAddress>().GetByIdAsync(addressId)
            ?? throw new Exception("Address not found");

        await _unitOfWork.GetRepository<PersonAddress>().SoftDeleteAsync(address);
    }

    public async Task<PersonAddressDto> SetDefaultAddressAsync(Guid addressId)
    {
        var address = await _unitOfWork.GetRepository<PersonAddress>().GetByIdAsync(addressId)
            ?? throw new Exception("Address not found");

        // Diğer varsayılan adresleri güncelle
        var defaultAddresses = await _unitOfWork.GetRepository<PersonAddress>()
            .GetWhere(a => a.PersonId == address.PersonId && a.IsDefault)
            .ToListAsync();

        foreach (var defaultAddress in defaultAddresses)
        {
            defaultAddress.IsDefault = false;
            await _unitOfWork.GetRepository<PersonAddress>().UpdateAsync(defaultAddress);
        }

        address.IsDefault = true;
        await _unitOfWork.GetRepository<PersonAddress>().UpdateAsync(address);
        return _mapper.Map<PersonAddressDto>(address);
    }

    // Contact operations
    public async Task<PersonContactDto> AddContactAsync(Guid personId, PersonContactDto contactDto)
    {
        var person = await _unitOfWork.GetRepository<Person>().GetByIdAsync(personId)
            ?? throw new Exception("Person not found");

        var contact = _mapper.Map<PersonContact>(contactDto);
        contact.PersonId = personId;

        if (contact.IsDefault || contact.IsPrimary)
        {
            // Diğer varsayılan/birincil kişileri güncelle
            var existingContacts = await _unitOfWork.GetRepository<PersonContact>()
                .GetWhere(c => c.PersonId == personId &&
                              (c.IsDefault || c.IsPrimary) &&
                              c.ContactType == contact.ContactType)
                .ToListAsync();

            foreach (var existingContact in existingContacts)
            {
                if (contact.IsDefault) existingContact.IsDefault = false;
                if (contact.IsPrimary) existingContact.IsPrimary = false;
                await _unitOfWork.GetRepository<PersonContact>().UpdateAsync(existingContact);
            }
        }

        await _unitOfWork.GetRepository<PersonContact>().AddAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }

    public async Task<PersonContactDto> UpdateContactAsync(Guid contactId, PersonContactDto contactDto)
    {
        var contact = await _unitOfWork.GetRepository<PersonContact>().GetByIdAsync(contactId)
            ?? throw new NotFoundException("Contact not found");

        // ID'yi güncelleme, sadece diğer özellikleri güncelle
        contact.ContactType = contactDto.ContactType;
        contact.ContactName = contactDto.ContactName;
        contact.ContactValue = contactDto.ContactValue;
        contact.CountryCode = contactDto.CountryCode;
        contact.AreaCode = contactDto.AreaCode;
        contact.IsDefault = contactDto.IsDefault;
        contact.IsPrimary = contactDto.IsPrimary;

        await _unitOfWork.GetRepository<PersonContact>().UpdateAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }

    public async Task DeleteContactAsync(Guid contactId)
    {
        var contact = await _unitOfWork.GetRepository<PersonContact>().GetByIdAsync(contactId)
            ?? throw new Exception("Contact not found");

        await _unitOfWork.GetRepository<PersonContact>().SoftDeleteAsync(contact);
    }

    public async Task<PersonContactDto> SetDefaultContactAsync(Guid contactId)
    {
        var contact = await _unitOfWork.GetRepository<PersonContact>().GetByIdAsync(contactId)
            ?? throw new Exception("Contact not found");

        // Diğer varsayılan kişileri güncelle
        var defaultContacts = await _unitOfWork.GetRepository<PersonContact>()
            .GetWhere(c => c.PersonId == contact.PersonId &&
                          c.IsDefault &&
                          c.ContactType == contact.ContactType)
            .ToListAsync();

        foreach (var defaultContact in defaultContacts)
        {
            defaultContact.IsDefault = false;
            await _unitOfWork.GetRepository<PersonContact>().UpdateAsync(defaultContact);
        }

        contact.IsDefault = true;
        await _unitOfWork.GetRepository<PersonContact>().UpdateAsync(contact);
        return _mapper.Map<PersonContactDto>(contact);
    }

    public async Task<List<PersonContactDto>> GetPersonContactsAsync(Guid personId)
    {
        var contacts = await _unitOfWork.GetRepository<PersonContact>()
            .GetWhere(c => c.PersonId == personId && !c.IsDeleted)
            .Select(c => new PersonContactDto
            {
                Id = c.Id,
                ContactType = c.ContactType,
                ContactName = c.ContactName,
                ContactValue = c.ContactValue,
                IsDefault = c.IsDefault,
                IsPrimary = c.IsPrimary
            })
            .OrderByDescending(c => c.IsPrimary)
            .ThenByDescending(c => c.IsDefault)
            .ThenBy(c => c.ContactType)
            .ToListAsync();

        return contacts;
    }

    public async Task<List<PersonAddressDto>> GetPersonAddressesAsync(Guid personId)
    {
        // Sadece gerekli alanları seç ve tek sorguda getir
        var addresses = await _unitOfWork.GetRepository<PersonAddress>()
            .GetWhere(a => a.PersonId == personId && !a.IsDeleted)
            .Select(a => new PersonAddressDto
            {
                Id = a.Id,
                AddressType = a.AddressType,
                AddressName = a.AddressName,
                AddressLine1 = a.AddressLine1,
                City = a.City,
                Country = a.Country,
                PostalCode = a.PostalCode,
                IsDefault = a.IsDefault
            })
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.AddressType)
            .ThenBy(a => a.AddressName)
            .ToListAsync();

        return addresses;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Person;
using Microsoft.Extensions.Logging;
using Wallet.Services.Exceptions;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("Person Management")]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;
    private readonly ILogger<PersonController> _logger;

    public PersonController(
        IPersonService personService,
        ILogger<PersonController> logger)
    {
        _personService = personService;
        _logger = logger;
    }

    /// <summary>
    /// Kişi bilgilerini getirir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/person/{id}
    /// 
    /// Örnek yanıt:
    /// 
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "dateOfBirth": "1990-01-15",
    ///         "gender": "Male",
    ///         "addresses": [
    ///             {
    ///                 "addressType": "Home",
    ///                 "addressLine1": "123 Main St",
    ///                 "city": "New York",
    ///                 "country": "USA",
    ///                 "postalCode": "10001",
    ///                 "isDefault": true
    ///             }
    ///         ],
    ///         "contacts": [
    ///             {
    ///                 "contactType": "Email",
    ///                 "contactValue": "john.doe@example.com",
    ///                 "isDefault": true
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    /// <param name="id">Kişi ID</param>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> GetPerson(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting person details for {PersonId}", id);
            
            var person = await _personService.GetPersonByIdAsync(id);
            if (person == null)
            {
                _logger.LogWarning("Person {PersonId} not found", id);
                return NotFound(new { message = "Person not found" });
            }
            
            _logger.LogInformation("Retrieved person details for {PersonId}", id);
            return Ok(person);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting person details for {PersonId}", id);
            return StatusCode(500, new { message = "Kişi bilgileri getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Yeni kişi oluşturur
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/person
    ///     {
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "dateOfBirth": "1990-01-15",
    ///         "gender": "Male",
    ///         "language": "en-US",
    ///         "timeZone": "America/New_York",
    ///         "currency": "USD"
    ///     }
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] PersonDto personDto)
    {
        try
        {
            _logger.LogInformation("Creating new person");
            
            var person = await _personService.CreatePersonAsync(personDto);
            
            _logger.LogInformation("Created person {PersonId}", person.Id);
            return CreatedAtAction(nameof(GetPerson), new { id = person.Id }, person);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create person: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating person");
            return StatusCode(500, new { message = "Kişi oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kişi bilgilerini günceller
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/person/{id}
    ///     {
    ///         "firstName": "John",
    ///         "lastName": "Doe",
    ///         "language": "en-US",
    ///         "timeZone": "America/New_York"
    ///     }
    /// </remarks>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PersonDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonDto>> UpdatePerson(Guid id, [FromBody] PersonDto personDto)
    {
        try
        {
            _logger.LogInformation("Updating person {PersonId}", id);
            
            var person = await _personService.UpdatePersonAsync(id, personDto);
            
            _logger.LogInformation("Updated person {PersonId}", id);
            return Ok(person);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Person {PersonId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating person {PersonId}", id);
            return StatusCode(500, new { message = "Kişi güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kişiyi siler (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting person {PersonId}", id);
            
            await _personService.DeletePersonAsync(id);
            
            _logger.LogInformation("Deleted person {PersonId}", id);
            return Ok(new { message = "Person deleted successfully" });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Person {PersonId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting person {PersonId}", id);
            return StatusCode(500, new { message = "Kişi silinirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Kişiye yeni adres ekler
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/person/{personId}/address
    ///     {
    ///         "addressType": "Home",
    ///         "addressLine1": "123 Main St",
    ///         "city": "New York",
    ///         "country": "USA",
    ///         "postalCode": "10001",
    ///         "isDefault": true
    ///     }
    /// </remarks>
    [HttpPost("{personId}/address")]
    [ProducesResponseType(typeof(PersonAddressDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonAddressDto>> AddAddress(Guid personId, [FromBody] PersonAddressDto addressDto)
    {
        try
        {
            _logger.LogInformation("Adding address for person {PersonId}", personId);
            
            var address = await _personService.AddAddressAsync(personId, addressDto);
            
            _logger.LogInformation("Added address {AddressId} for person {PersonId}", 
                address.Id, personId);
            return CreatedAtAction(nameof(GetPerson), new { id = personId }, address);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Person {PersonId} not found for adding address", personId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding address for person {PersonId}", personId);
            return StatusCode(500, new { message = "Adres eklenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Adres bilgilerini günceller
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/person/address/{addressId}
    ///     {
    ///         "addressLine1": "456 New St",
    ///         "city": "Boston",
    ///         "isDefault": true
    ///     }
    /// </remarks>
    [HttpPut("address/{addressId}")]
    [ProducesResponseType(typeof(PersonAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonAddressDto>> UpdateAddress(Guid addressId, [FromBody] PersonAddressDto addressDto)
    {
        try
        {
            _logger.LogInformation("Updating address {AddressId}", addressId);
            
            var address = await _personService.UpdateAddressAsync(addressId, addressDto);
            
            _logger.LogInformation("Updated address {AddressId}", addressId);
            return Ok(address);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Address {AddressId} not found for update", addressId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating address {AddressId}", addressId);
            return StatusCode(500, new { message = "Adres güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Adresi siler
    /// </summary>
    [HttpDelete("address/{addressId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAddress(Guid addressId)
    {
        try
        {
            _logger.LogInformation("Deleting address {AddressId}", addressId);
            
            await _personService.DeleteAddressAsync(addressId);
            
            _logger.LogInformation("Deleted address {AddressId}", addressId);
            return Ok(new { message = "Address deleted successfully" });
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Address {AddressId} not found for deletion", addressId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting address {AddressId}", addressId);
            return StatusCode(500, new { message = "Adres silinirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Adresi varsayılan olarak ayarlar
    /// </summary>
    [HttpPut("address/{addressId}/default")]
    [ProducesResponseType(typeof(PersonAddressDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonAddressDto>> SetDefaultAddress(Guid addressId)
    {
        try
        {
            var address = await _personService.SetDefaultAddressAsync(addressId);
            return Ok(address);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kişiye yeni iletişim bilgisi ekler
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/person/{personId}/contact
    ///     {
    ///         "contactType": "Email",
    ///         "contactValue": "john.doe@example.com",
    ///         "isDefault": true,
    ///         "isPrimary": true
    ///     }
    /// </remarks>
    [HttpPost("{personId}/contact")]
    [ProducesResponseType(typeof(PersonContactDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonContactDto>> AddContact(Guid personId, [FromBody] PersonContactDto contactDto)
    {
        try
        {
            var contact = await _personService.AddContactAsync(personId, contactDto);
            return CreatedAtAction(nameof(GetPerson), new { id = personId }, contact);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// İletişim bilgisini günceller
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/person/contact/{contactId}
    ///     {
    ///         "contactValue": "new.email@example.com",
    ///         "isDefault": true
    ///     }
    /// </remarks>
    [HttpPut("contact/{contactId}")]
    [ProducesResponseType(typeof(PersonContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonContactDto>> UpdateContact(Guid contactId, [FromBody] PersonContactDto contactDto)
    {
        try
        {
            var contact = await _personService.UpdateContactAsync(contactId, contactDto);
            return Ok(contact);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// İletişim bilgisini siler
    /// </summary>
    [HttpDelete("contact/{contactId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteContact(Guid contactId)
    {
        try
        {
            await _personService.DeleteContactAsync(contactId);
            return Ok(new { message = "Contact deleted successfully" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// İletişim bilgisini varsayılan olarak ayarlar
    /// </summary>
    [HttpPut("contact/{contactId}/default")]
    [ProducesResponseType(typeof(PersonContactDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonContactDto>> SetDefaultContact(Guid contactId)
    {
        try
        {
            var contact = await _personService.SetDefaultContactAsync(contactId);
            return Ok(contact);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kişinin iletişim bilgilerini getirir
    /// </summary>
    /// <remarks>
    /// Örnek yanıt:
    /// 
    ///     [
    ///         {
    ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "contactType": "Email",
    ///             "contactValue": "john.doe@example.com",
    ///             "isDefault": true,
    ///             "isPrimary": true
    ///         },
    ///         {
    ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///             "contactType": "Phone",
    ///             "contactValue": "+905551234567",
    ///             "isDefault": false,
    ///             "isPrimary": false
    ///         }
    ///     ]
    /// </remarks>
    /// <param name="personId">Kişi ID</param>
    /// <returns>İletişim bilgileri listesi</returns>
    [HttpGet("{personId}/contacts")]
    [ProducesResponseType(typeof(List<PersonContactDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PersonContactDto>>> GetPersonContacts(Guid personId)
    {
        try
        {
            var contacts = await _personService.GetPersonContactsAsync(personId);
            return Ok(contacts);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Kişinin adres bilgilerini getirir
    /// </summary>
    /// <remarks>
    /// Örnek yanıt:
    /// 
    ///     [
    ///         {
    ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///             "addressType": "Home",
    ///             "addressName": "Ev Adresi",
    ///             "addressLine1": "Bağdat Caddesi No:123",
    ///             "city": "Istanbul",
    ///             "country": "Turkey",
    ///             "postalCode": "34744",
    ///             "isDefault": true
    ///         },
    ///         {
    ///             "id": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    ///             "addressType": "Work",
    ///             "addressName": "İş Adresi",
    ///             "addressLine1": "Levent Plaza",
    ///             "city": "Istanbul",
    ///             "country": "Turkey",
    ///             "postalCode": "34330",
    ///             "isDefault": false
    ///         }
    ///     ]
    /// </remarks>
    /// <param name="personId">Kişi ID</param>
    /// <returns>Adres bilgileri listesi</returns>
    [HttpGet("{personId}/addresses")]
    [ProducesResponseType(typeof(List<PersonAddressDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<PersonAddressDto>>> GetPersonAddresses(Guid personId)
    {
        try
        {
            var addresses = await _personService.GetPersonAddressesAsync(personId);
            return Ok(addresses);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
} 
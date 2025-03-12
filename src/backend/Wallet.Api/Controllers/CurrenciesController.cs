using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Wallet.Services.Exceptions;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Currencies")]
public class CurrenciesController : ControllerBase
{
    private readonly ICurrencyService _currencyService;
    private readonly ILogger<CurrenciesController> _logger;

    public CurrenciesController(
        ICurrencyService currencyService,
        ILogger<CurrenciesController> logger)
    {
        _currencyService = currencyService;
        _logger = logger;
    }

    /// <summary>
    /// Gets all currencies
    /// </summary>
    /// <param name="includeInactive">Whether to include inactive currencies</param>
    /// <returns>List of currencies</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<CurrencyDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
    {
        try
        {
            _logger.LogInformation("Getting all currencies, includeInactive: {IncludeInactive}", includeInactive);
            var currencies = await _currencyService.GetAllCurrenciesAsync(includeInactive);
            _logger.LogInformation("Retrieved {Count} currencies", currencies.Count);
            return Ok(currencies);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currencies");
            return StatusCode(500, new { message = "Para birimleri getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Gets a currency by ID
    /// </summary>
    /// <param name="id">Currency ID</param>
    /// <returns>Currency details</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting currency {CurrencyId}", id);
            var currency = await _currencyService.GetCurrencyByIdAsync(id);
            _logger.LogInformation("Retrieved currency {CurrencyId}", id);
            return Ok(currency);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency {CurrencyId} not found", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currency {CurrencyId}", id);
            return StatusCode(500, new { message = "Para birimi getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Gets a currency by code
    /// </summary>
    /// <param name="code">Currency code (e.g., USD, EUR)</param>
    /// <returns>Currency details</returns>
    [HttpGet("code/{code}")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByCode(string code)
    {
        try
        {
            _logger.LogInformation("Getting currency with code {CurrencyCode}", code);
            var currency = await _currencyService.GetCurrencyByCodeAsync(code);
            _logger.LogInformation("Retrieved currency with code {CurrencyCode}", code);
            return Ok(currency);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency with code {CurrencyCode} not found", code);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting currency with code {CurrencyCode}", code);
            return StatusCode(500, new { message = "Para birimi getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Gets the default currency
    /// </summary>
    /// <returns>Default currency details</returns>
    [HttpGet("default")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDefault()
    {
        try
        {
            _logger.LogInformation("Getting default currency");
            var currency = await _currencyService.GetDefaultCurrencyAsync();
            _logger.LogInformation("Retrieved default currency: {CurrencyCode}", currency.Code);
            return Ok(currency);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Default currency not found");
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting default currency");
            return StatusCode(500, new { message = "Varsayılan para birimi getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Creates a new currency
    /// </summary>
    /// <param name="currencyDto">Currency details</param>
    /// <returns>Created currency</returns>
    [HttpPost]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyDto currencyDto)
    {
        try
        {
            _logger.LogInformation("Creating new currency with code {CurrencyCode}", currencyDto.Code);
            var created = await _currencyService.CreateCurrencyAsync(currencyDto);
            _logger.LogInformation("Created currency {CurrencyId} with code {CurrencyCode}", created.Id, created.Code);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create currency: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating currency with code {CurrencyCode}", currencyDto.Code);
            return StatusCode(500, new { message = "Para birimi oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Updates a currency
    /// </summary>
    /// <param name="id">Currency ID</param>
    /// <param name="currencyDto">Updated currency details</param>
    /// <returns>Updated currency</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCurrencyDto currencyDto)
    {
        try
        {
            _logger.LogInformation("Updating currency {CurrencyId}", id);
            var updated = await _currencyService.UpdateCurrencyAsync(id, currencyDto);
            _logger.LogInformation("Updated currency {CurrencyId}", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency {CurrencyId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to update currency {CurrencyId}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating currency {CurrencyId}", id);
            return StatusCode(500, new { message = "Para birimi güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Deletes a currency
    /// </summary>
    /// <param name="id">Currency ID</param>
    /// <returns>No content</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting currency {CurrencyId}", id);
            await _currencyService.DeleteCurrencyAsync(id);
            _logger.LogInformation("Deleted currency {CurrencyId}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency {CurrencyId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to delete currency {CurrencyId}: {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting currency {CurrencyId}", id);
            return StatusCode(500, new { message = "Para birimi silinirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Sets a currency as the default
    /// </summary>
    /// <param name="id">Currency ID</param>
    /// <returns>Updated currency</returns>
    [HttpPost("{id}/set-default")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        try
        {
            _logger.LogInformation("Setting currency {CurrencyId} as default", id);
            var updated = await _currencyService.SetDefaultCurrencyAsync(id);
            _logger.LogInformation("Set currency {CurrencyId} as default", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency {CurrencyId} not found for setting as default", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting currency {CurrencyId} as default", id);
            return StatusCode(500, new { message = "Para birimi varsayılan olarak ayarlanırken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Updates a currency's exchange rate
    /// </summary>
    /// <param name="id">Currency ID</param>
    /// <param name="exchangeRate">New exchange rate</param>
    /// <returns>Updated currency</returns>
    [HttpPut("{id}/exchange-rate")]
    [ProducesResponseType(typeof(CurrencyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExchangeRate(Guid id, [FromBody] decimal exchangeRate)
    {
        try
        {
            _logger.LogInformation("Updating exchange rate for currency {CurrencyId} to {ExchangeRate}", id, exchangeRate);
            var updated = await _currencyService.UpdateExchangeRateAsync(id, exchangeRate);
            _logger.LogInformation("Updated exchange rate for currency {CurrencyId}", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Currency {CurrencyId} not found for updating exchange rate", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating exchange rate for currency {CurrencyId}", id);
            return StatusCode(500, new { message = "Para birimi kuru güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Gets all exchange rates
    /// </summary>
    /// <returns>Dictionary of currency codes and exchange rates</returns>
    [HttpGet("exchange-rates")]
    [ProducesResponseType(typeof(Dictionary<string, decimal>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExchangeRates()
    {
        try
        {
            _logger.LogInformation("Getting all exchange rates");
            var rates = await _currencyService.GetExchangeRatesAsync();
            _logger.LogInformation("Retrieved {Count} exchange rates", rates.Count);
            return Ok(rates);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting exchange rates");
            return StatusCode(500, new { message = "Döviz kurları getirilirken bir hata oluştu" });
        }
    }
} 
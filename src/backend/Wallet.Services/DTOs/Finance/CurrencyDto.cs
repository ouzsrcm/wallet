namespace Wallet.Services.DTOs.Finance;

/// <summary>
/// Currency information DTO
/// </summary>
/// <example>
/// {
///   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
///   "code": "USD",
///   "name": "US Dollar",
///   "symbol": "$",
///   "flag": "🇺🇸",
///   "isActive": true,
///   "isDefault": false,
///   "decimalPlaces": 2,
///   "format": "{1}{0}",
///   "exchangeRate": 1.0,
///   "lastUpdated": "2023-01-01T00:00:00Z"
/// }
/// </example>
public class CurrencyDto
{
    public Guid Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Symbol { get; set; }
    public string? Flag { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int DecimalPlaces { get; set; } = 2;
    public string? Format { get; set; }
    public decimal? ExchangeRate { get; set; }
    public DateTime? LastUpdated { get; set; }
}

/// <summary>
/// DTO for creating a new currency
/// </summary>
public class CreateCurrencyDto
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public required string Symbol { get; set; }
    public string? Flag { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int DecimalPlaces { get; set; } = 2;
    public string? Format { get; set; }
    public decimal? ExchangeRate { get; set; }
}

/// <summary>
/// DTO for updating an existing currency
/// </summary>
public class UpdateCurrencyDto
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }
    public string? Flag { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDefault { get; set; }
    public int? DecimalPlaces { get; set; }
    public string? Format { get; set; }
    public decimal? ExchangeRate { get; set; }
} 
using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class Currency : SoftDeleteEntity
{
    public required string Code { get; set; }  // ISO 4217 currency code (e.g., USD, EUR, TRY)
    public required string Name { get; set; }  // Full name (e.g., US Dollar, Euro, Turkish Lira)
    public required string Symbol { get; set; } // Currency symbol (e.g., $, €, ₺)
    public string? Flag { get; set; }  // Flag emoji or image URL
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    public int DecimalPlaces { get; set; } = 2;
    public string? Format { get; set; }  // Format string (e.g., "{0} {1}" for "10 $" or "{1}{0}" for "$10")
    public decimal? ExchangeRate { get; set; }  // Exchange rate to base currency
    public DateTime? LastUpdated { get; set; }  // Last time exchange rate was updated
} 
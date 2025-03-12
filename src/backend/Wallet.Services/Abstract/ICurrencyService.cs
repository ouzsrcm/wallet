using Wallet.Services.DTOs.Finance;

namespace Wallet.Services.Abstract;

public interface ICurrencyService
{
    Task<CurrencyDto> GetCurrencyByIdAsync(Guid id);
    Task<CurrencyDto> GetCurrencyByCodeAsync(string code);
    Task<List<CurrencyDto>> GetAllCurrenciesAsync(bool includeInactive = false);
    Task<CurrencyDto> GetDefaultCurrencyAsync();
    Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyDto currencyDto);
    Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyDto currencyDto);
    Task DeleteCurrencyAsync(Guid id);
    Task<CurrencyDto> SetDefaultCurrencyAsync(Guid id);
    Task<CurrencyDto> UpdateExchangeRateAsync(Guid id, decimal exchangeRate);
    Task<Dictionary<string, decimal>> GetExchangeRatesAsync();
} 
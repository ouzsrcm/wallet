using walletv2.Data.Entities.Objects;
using walletv2.Data.Proxies;
using walletv2.Data.Repositories;

namespace walletv2.Data.Services;

public interface ICurrencyService
{
    /// <summary>
    /// updates daily exchange rates from the currency proxy service.
    /// </summary>
    /// <returns></returns>
    Task<bool> UpdateDailyRates();
}

public class CurrencyService : ICurrencyService
{
    private readonly IBaseRepository<Currency> _currencyRepository;
    private readonly IBaseRepository<ExchangeRate> _exchangeRateRepository;
    private readonly IBaseRepository<ExchangeRateType> _exchangeRateTypeRepository;

    private readonly ICurrencyProxy currencyProxy;

    public CurrencyService(IBaseRepository<Currency> _currencyRepository,
        IBaseRepository<ExchangeRate> _exchangeRateRepository,
        IBaseRepository<ExchangeRateType> _exchangeRateTypeRepository,
        ICurrencyProxy currencyProxy
        )
    {
        this._currencyRepository = _currencyRepository ?? throw new ArgumentNullException(nameof(_currencyRepository));
        this._exchangeRateRepository = _exchangeRateRepository ?? throw new ArgumentNullException(nameof(_exchangeRateRepository));
        this._exchangeRateTypeRepository = _exchangeRateTypeRepository ?? throw new ArgumentNullException(nameof(_exchangeRateTypeRepository));

        this.currencyProxy = currencyProxy ?? throw new ArgumentNullException(nameof(currencyProxy));
    }


    /// <summary>
    /// updates daily exchange rates from the currency proxy service.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<bool> UpdateDailyRates()
    {
        var rates = await currencyProxy.GetRates();
        if (rates is null || rates.Currencies is null || rates.Currencies.Count < 1)
            throw new InvalidOperationException("No exchange rates found to update.");

        await CreateExchangeRateTypes();

        await CreateCurrencies();

        var rateTypes = await _exchangeRateTypeRepository.AnyAsync(x => !x.isDeleted);
        if (!rateTypes)
            throw new InvalidOperationException("Exchange rate types must be created before creating currencies.");

        var existCurrencies = await _currencyRepository.AnyAsync(x => !x.isDeleted);
        if (!existCurrencies)
            throw new InvalidOperationException("Currencies must be created before creating exchange rates.");

        foreach (var rate in rates.Currencies)
        {
            var currencyId = (await _currencyRepository.FindAsync(x => x.CurrencyCode == rate.CurrencyCode)).FirstOrDefault()?.Id;
            var exchangeRateTypeId = (await _exchangeRateTypeRepository.FindAsync(x => x.TypeName == "ForexBuying")).FirstOrDefault()?.Id;

            if (currencyId == null || exchangeRateTypeId == null)
                throw new InvalidOperationException($"Currency or Exchange Rate Type not found for {rate.CurrencyCode}.");

            var _tempRate = decimal.TryParse(rate.ForexBuying,
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out var forexBuying);

            if (_tempRate == false)
                throw new InvalidOperationException($"Invalid ForexBuying rate for {rate.CurrencyCode}.");

            var model = new ExchangeRate
            {
                CurrencyId = currencyId.Value,
                Rate = forexBuying,
                RateDate = DateTime.UtcNow,
                ExchangeRateTypeId = exchangeRateTypeId.Value
            };
            await _exchangeRateRepository.AddAsync(model);
        }

        await _exchangeRateRepository.SaveChangesAsync();

        return await Task.FromResult(true);
    }

    /// <summary>
    /// create currencies if not exists.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> CreateCurrencies()
    {
        var any = await _currencyRepository.AnyAsync(x => !x.isDeleted);
        if (any)
            return true; // Currencies already exist, no need to create again.

        var rates = await currencyProxy.GetRates();
        if (rates == null)
            throw new InvalidOperationException("No exchange rates found to create currencies.");

        foreach (var rate in rates.Currencies)
            await _currencyRepository.AddAsync(new Currency
            {
                CurrencyCode = rate.CurrencyCode,
                CurrencyName = rate.CurrencyCode,
                IsLocal = false,
            });
        await _currencyRepository.AddAsync(new Currency() { IsLocal = true, CurrencyCode = "TRY", CurrencyName = "Türk Lirası" });
        await _currencyRepository.SaveChangesAsync();

        return await Task.FromResult(true);
    }

    private async Task<bool> CreateExchangeRateTypes()
    {
        var any = await _exchangeRateTypeRepository.AnyAsync(x => !x.isDeleted);
        if (any)
            return true; // Exchange rate types already exist, no need to create again.

        var exchangeRateTypes = new List<ExchangeRateType>
        {
            new ExchangeRateType { TypeName = "ForexBuying" },
            new ExchangeRateType { TypeName = "ForexSelling" },
            new ExchangeRateType { TypeName = "BanknoteBuying" },
            new ExchangeRateType { TypeName = "BanknoteSelling" }
        };

        foreach (var type in exchangeRateTypes)
            await _exchangeRateTypeRepository.AddAsync(type);

        await _exchangeRateTypeRepository.SaveChangesAsync();
        return true;
    }

}

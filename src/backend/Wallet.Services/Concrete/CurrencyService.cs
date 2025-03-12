using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Wallet.Services.Exceptions;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.Concrete;

public class CurrencyService : ICurrencyService
{
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CurrencyService(IFinanceUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CurrencyDto> GetCurrencyByIdAsync(Guid id)
    {
        var currency = await _unitOfWork.GetRepository<Currency>().GetByIdAsync(id)
            ?? throw new NotFoundException($"Currency with ID {id} not found");

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<CurrencyDto> GetCurrencyByCodeAsync(string code)
    {
        var currency = await _unitOfWork.GetRepository<Currency>()
            .GetSingleAsync(c => c.Code == code && !c.IsDeleted)
            ?? throw new NotFoundException($"Currency with code {code} not found");

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<List<CurrencyDto>> GetAllCurrenciesAsync(bool includeInactive = false)
    {
        var query = _unitOfWork.GetRepository<Currency>().GetWhere(c => !c.IsDeleted);
        
        if (!includeInactive)
        {
            query = query.Where(c => c.IsActive);
        }

        var currencies = await query.OrderBy(c => c.Name).Select(c => new CurrencyDto
        {
            Id = c.Id,
            Code = c.Code,
            Name = c.Name,
            Symbol = c.Symbol,
            Flag = c.Flag,
            IsActive = c.IsActive,
            IsDefault = c.IsDefault,
            DecimalPlaces = c.DecimalPlaces,
            Format = c.Format,
            ExchangeRate = c.ExchangeRate,
        }).ToListAsync();
        
        return currencies;
    }

    public async Task<CurrencyDto> GetDefaultCurrencyAsync()
    {
        var currency = await _unitOfWork.GetRepository<Currency>()
            .GetSingleAsync(c => c.IsDefault && !c.IsDeleted)
            ?? throw new NotFoundException("Default currency not found");

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<CurrencyDto> CreateCurrencyAsync(CreateCurrencyDto currencyDto)
    {
        // Check if currency with the same code already exists
        var existingCurrency = await _unitOfWork.GetRepository<Currency>()
            .GetSingleAsync(c => c.Code == currencyDto.Code && !c.IsDeleted);

        if (existingCurrency != null)
        {
            throw new BadRequestException($"Currency with code {currencyDto.Code} already exists");
        }

        // If this is the first currency or it's set as default, ensure no other default exists
        if (currencyDto.IsDefault)
        {
            await EnsureNoOtherDefaultCurrencyAsync();
        }

        var currency = _mapper.Map<Currency>(currencyDto);
        currency.LastUpdated = DateTime.UtcNow;

        await _unitOfWork.GetRepository<Currency>().AddAsync(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<CurrencyDto> UpdateCurrencyAsync(Guid id, UpdateCurrencyDto currencyDto)
    {
        var currency = await _unitOfWork.GetRepository<Currency>().GetByIdAsync(id)
            ?? throw new NotFoundException($"Currency with ID {id} not found");

        // If setting as default, ensure no other default exists
        if (currencyDto.IsDefault.HasValue && currencyDto.IsDefault.Value && !currency.IsDefault)
        {
            await EnsureNoOtherDefaultCurrencyAsync();
        }

        _mapper.Map(currencyDto, currency);

        // Update LastUpdated if exchange rate is updated
        if (currencyDto.ExchangeRate.HasValue)
        {
            currency.LastUpdated = DateTime.UtcNow;
        }

        await _unitOfWork.GetRepository<Currency>().UpdateAsync(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task DeleteCurrencyAsync(Guid id)
    {
        var currency = await _unitOfWork.GetRepository<Currency>().GetByIdAsync(id)
            ?? throw new NotFoundException($"Currency with ID {id} not found");

        // Don't allow deleting the default currency
        if (currency.IsDefault)
        {
            throw new BadRequestException("Cannot delete the default currency");
        }

        await _unitOfWork.GetRepository<Currency>().SoftDeleteAsync(currency);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<CurrencyDto> SetDefaultCurrencyAsync(Guid id)
    {
        var currency = await _unitOfWork.GetRepository<Currency>().GetByIdAsync(id)
            ?? throw new NotFoundException($"Currency with ID {id} not found");

        if (currency.IsDefault)
        {
            return _mapper.Map<CurrencyDto>(currency);
        }

        await EnsureNoOtherDefaultCurrencyAsync();

        currency.IsDefault = true;
        await _unitOfWork.GetRepository<Currency>().UpdateAsync(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<CurrencyDto> UpdateExchangeRateAsync(Guid id, decimal exchangeRate)
    {
        var currency = await _unitOfWork.GetRepository<Currency>().GetByIdAsync(id)
            ?? throw new NotFoundException($"Currency with ID {id} not found");

        currency.ExchangeRate = exchangeRate;
        currency.LastUpdated = DateTime.UtcNow;

        await _unitOfWork.GetRepository<Currency>().UpdateAsync(currency);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<CurrencyDto>(currency);
    }

    public async Task<Dictionary<string, decimal>> GetExchangeRatesAsync()
    {
        var currencies = await _unitOfWork.GetRepository<Currency>()
            .GetWhere(c => c.IsActive && !c.IsDeleted && c.ExchangeRate.HasValue)
            .ToListAsync();

        return currencies
            .Where(c => c.ExchangeRate.HasValue)
            .ToDictionary(c => c.Code, c => c.ExchangeRate!.Value);
    }

    private async Task EnsureNoOtherDefaultCurrencyAsync()
    {
        var defaultCurrency = await _unitOfWork.GetRepository<Currency>()
            .GetSingleAsync(c => c.IsDefault && !c.IsDeleted);

        if (defaultCurrency != null)
        {
            defaultCurrency.IsDefault = false;
            await _unitOfWork.GetRepository<Currency>().UpdateAsync(defaultCurrency);
        }
    }
} 
using walletv2.Data.Entities.DataTransferObjects;
using walletv2.Data.Entities.Objects;
using walletv2.Data.Repositories;

namespace walletv2.Data.Services;

public interface IWalletService
{
    /// <summary>
    /// distribute cashflow to multiple items.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task DisributeCashflowAsync(DistributeCashflowDto model);

    /// <summary>
    /// create a new account for a user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Guid> CreateAccountAsync(CreateAccountDto model);

    /// <summary>
    /// create a new income/expense record.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    Task<Guid> CreateIncomeExpenseAsync(CreateIncomeExpenseDto model);
}

/// <summary>
/// all of cashflow operations.
/// </summary>
public class WalletService : IWalletService
{
    private readonly IBaseRepository<CashflowType> _cashflowTypeRepository;
    private readonly IBaseRepository<Cashflow> _cashflowRepository;
    private readonly IBaseRepository<Account> _accountRepository;
    private readonly IBaseRepository<Currency> _currencyRepository;
    private readonly IBaseRepository<ExchangeRate> _exchangeRateRepository;
    private readonly IBaseRepository<ExchangeRateType> _exchangeRateTypeRepository;
    private readonly IBaseRepository<CashflowDocument> _cashflowDocumentRepository;
    private readonly IBaseRepository<IncomeExpense> _incomeExpenseRepository;

    /// <summary>
    /// default expense types for cashflow.
    /// </summary>
    public static string[] DefaultCashflowTypes =
    [
        "Income",
        "Expense",
        "Transfer"
    ];

    public WalletService(IBaseRepository<CashflowType> _cashflowTypeRepository,
                            IBaseRepository<Cashflow> _cashflowRepository,
                            IBaseRepository<Account> _accountRepository,
                            IBaseRepository<Currency> _currencyRepository,
                            IBaseRepository<ExchangeRate> _exchangeRateRepository,
                            IBaseRepository<ExchangeRateType> _exchangeRateTypeRepository,
                            IBaseRepository<CashflowDocument> _cashflowDocumentRepository,
                            IBaseRepository<IncomeExpense> _incomeExpenseRepository)
    {
        this._cashflowTypeRepository = _cashflowTypeRepository ?? throw new ArgumentNullException(nameof(_cashflowTypeRepository));
        this._cashflowRepository = _cashflowRepository ?? throw new ArgumentNullException(nameof(_cashflowRepository));
        this._accountRepository = _accountRepository ?? throw new ArgumentNullException(nameof(_accountRepository));
        this._currencyRepository = _currencyRepository ?? throw new ArgumentNullException(nameof(_currencyRepository));
        this._exchangeRateRepository = _exchangeRateRepository ?? throw new ArgumentNullException(nameof(_exchangeRateRepository));
        this._incomeExpenseRepository = _incomeExpenseRepository ?? throw new ArgumentNullException(nameof(_incomeExpenseRepository));
        this._exchangeRateTypeRepository = _exchangeRateTypeRepository ?? throw new ArgumentNullException(nameof(_exchangeRateTypeRepository));
        this._cashflowDocumentRepository = _cashflowDocumentRepository ?? throw new ArgumentNullException(nameof(_cashflowDocumentRepository));
    }

    /// <summary>
    /// create a new account for a user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public async Task<Guid> CreateAccountAsync(CreateAccountDto model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.CurrencyId == Guid.Empty)
            throw new ArgumentException("CurrencyId cannot be empty.", nameof(model.CurrencyId));

        if (model.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(model.UserId));

        if (string.IsNullOrWhiteSpace(model.Name) || model.Name.Length < 3 || model.Name.Length > 100)
            throw new ArgumentException("Name must be between 3 and 100 characters.", nameof(model.Name));

        var res = await _accountRepository.AddAsync(new Account
        {
            CurrencyId = model.CurrencyId,
            UserId = model.UserId,
            Name = model.Name
        });
        await _accountRepository.SaveChangesAsync();

        return res.Id;
    }

    /// <summary>
    /// distribute cashflow to multiple items.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task DisributeCashflowAsync(DistributeCashflowDto model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.Items == null || !model.Items.Any())
            throw new ArgumentException("Items cannot be null or empty.", nameof(model.Items));

        foreach (var item in model.Items)
        {
            if (await createCashflowAsync(new Cashflow
            {
                CashflowTypeId = item.CashflowTypeId,
                UserId = model.UserId,
                Credit = item.Credit,
                Debit = item.Debit,
                Description = item.Description,
                CurrencyId = item.CurrencyId
            }, saveInline: false) == null)
                throw new InvalidOperationException("Failed to create cashflow.");
        }
        await _cashflowRepository.SaveChangesAsync();
    }

    /// <summary>
    /// creates a new cashflow record.
    /// </summary>
    /// <param name="cashflow"></param>
    /// <param name="saveInline">saves row by row to database.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<Cashflow> createCashflowAsync(Cashflow cashflow, bool saveInline = false)
    {
        if (cashflow == null) throw new ArgumentNullException(nameof(cashflow));
        if (cashflow.CashflowTypeId == Guid.Empty) throw new ArgumentException("CashflowTypeId cannot be empty.", nameof(cashflow.CashflowTypeId));
        if (cashflow.UserId == Guid.Empty) throw new ArgumentException("UserId cannot be empty.", nameof(cashflow.UserId));
        if (cashflow.Credit < 0) throw new ArgumentException("Credit cannot be negative.", nameof(cashflow.Credit));

        var document = await _cashflowDocumentRepository.AddAsync(new CashflowDocument
        {
            CashflowId = cashflow.Id,
            DocumentNumber = Guid.NewGuid().ToString() // TODO: Generate a proper document number
        });
        if (document == null)
            throw new InvalidOperationException("Failed to create cashflow document.");

        cashflow.CashflowDocumentId = document.Id;

        var currencyRes = await _currencyRepository.FindAsync(x => x.Id == cashflow.CurrencyId);
        if (currencyRes == null)
            throw new ArgumentException("CurrencyId is invalid.", nameof(cashflow.CurrencyId));

        var currency = currencyRes.FirstOrDefault();
        if (currency == null)
            throw new ArgumentException("CurrencyId is invalid.", nameof(cashflow.CurrencyId));

        var defaultRateTypeRes = (await _exchangeRateTypeRepository.FindAsync(x => x.TypeName == CurrencyService.DefaultRateTypeName))?.FirstOrDefault();
        if (defaultRateTypeRes == null)
            throw new InvalidOperationException("Default exchange rate type not found.");

        var rateRes = await _exchangeRateRepository.FindAsync(x => x.CurrencyId == cashflow.CurrencyId
                                                                && x.ExchangeRateTypeId == defaultRateTypeRes.Id
                                                                && x.RateDate == DateTime.UtcNow.Date);
        if (rateRes == null)
            throw new InvalidOperationException($"Exchange rate not found for the specified currency. {DateTime.UtcNow}");

        var rate = rateRes.FirstOrDefault();
        if (rate == null)
            throw new InvalidOperationException("Exchange rate not found for the specified currency.");

        cashflow.CurrencyRate = rate.Rate;

        if (!currency.IsLocal)
        {
            cashflow.DebitTRY = cashflow.Debit * rate.Rate;
            cashflow.CreditTRY = cashflow.Credit * rate.Rate;
        }

        var res = await _cashflowRepository.AddAsync(cashflow);

        if (saveInline)
            await _cashflowRepository.SaveChangesAsync();

        return res ?? throw new InvalidOperationException("Failed to create cashflow.");
    }

    /// <summary>
    /// create a new income/expense record.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<Guid> CreateIncomeExpenseAsync(CreateIncomeExpenseDto model)
    {
        if (model == null)
            throw new ArgumentNullException(nameof(model));

        if (model.UserId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty.", nameof(model.UserId));

        if (model.IncomeExpenseTypeId == Guid.Empty)
            throw new ArgumentException("IncomeExpenseTypeId cannot be empty.", nameof(model.IncomeExpenseTypeId));

        if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Length < 3 || model.Description.Length > 500)
            throw new ArgumentException("Description must be between 3 and 500 characters.", nameof(model.Description));

        var res = await _incomeExpenseRepository.AddAsync(new IncomeExpense
        {
            UserId = model.UserId,
            IncomeExpenseTypeId = model.IncomeExpenseTypeId,
            ParentId = model.ParentId,
            Description = model.Description,
            Icon = model.Icon
        });

        await _incomeExpenseRepository.SaveChangesAsync();

        return res.Id;
    }
}

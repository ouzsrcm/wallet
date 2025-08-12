using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Security.Cryptography;
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

    /// <summary>
    /// list of the income and expense records for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IncomeExpenseListDto> GetListOfIncomeExpenseAsync(Guid userId);

    /// <summary>
    /// list of all accounts.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IncomeExpenseTypeListDto> GetIncomeExpenseTypeListAsync();

    /// <summary>
    /// list of all cashflow types for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<CashflowListDto> GetCashflowListAsync(GetCashflowListDto model);
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
    private readonly IBaseRepository<IncomeExpenseType> _incomeExpenseTypeRepository;

    private readonly IUserService _userService;

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
                            IBaseRepository<IncomeExpense> _incomeExpenseRepository,
                            IBaseRepository<IncomeExpenseType> _incomeExpenseTypeRepository,
                            IUserService _userService)
    {
        this._cashflowTypeRepository = _cashflowTypeRepository ?? throw new ArgumentNullException(nameof(_cashflowTypeRepository));
        this._cashflowRepository = _cashflowRepository ?? throw new ArgumentNullException(nameof(_cashflowRepository));
        this._accountRepository = _accountRepository ?? throw new ArgumentNullException(nameof(_accountRepository));
        this._currencyRepository = _currencyRepository ?? throw new ArgumentNullException(nameof(_currencyRepository));
        this._exchangeRateRepository = _exchangeRateRepository ?? throw new ArgumentNullException(nameof(_exchangeRateRepository));
        this._incomeExpenseRepository = _incomeExpenseRepository ?? throw new ArgumentNullException(nameof(_incomeExpenseRepository));
        this._exchangeRateTypeRepository = _exchangeRateTypeRepository ?? throw new ArgumentNullException(nameof(_exchangeRateTypeRepository));
        this._cashflowDocumentRepository = _cashflowDocumentRepository ?? throw new ArgumentNullException(nameof(_cashflowDocumentRepository));
        this._incomeExpenseTypeRepository = _incomeExpenseTypeRepository;

        this._userService = _userService ?? throw new ArgumentNullException(nameof(_userService));
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

        var cfTypes = await _cashflowTypeRepository.FindAsync(x => !x.isDeleted);
        if (cfTypes == null || !cfTypes.Any())
            throw new InvalidOperationException("No cashflow types found.");

        foreach (var item in model.Items)
        {
            var itemCfType = await cfTypes.AnyAsync(x => x.Id == item.CashflowTypeId) ?
                cfTypes.FirstOrDefault(x => x.Id == item.CashflowTypeId) :
                null;

            if (itemCfType == null)
                throw new ArgumentException($"CashflowTypeId {item.CashflowTypeId} is invalid.", nameof(item.CashflowTypeId));

            if (itemCfType.Name == "Income" && item.Credit < 0)
                throw new ArgumentException("Credit cannot be negative for non-Transfer types.", nameof(item.Credit));

            if (itemCfType.Name == "Expense" && item.Debit < 0)
                throw new ArgumentException("Debit cannot be negative for non-Transfer types.", nameof(item.Debit));

            if (itemCfType.Name == "Transfer")
                await moneyTransfer(item.Credit, item.FromAccountId, item.ToAccountId);
            else
                await createCashflowAsync(new Cashflow
                {
                    CashflowTypeId = item.CashflowTypeId,
                    UserId = model.UserId,
                    Credit = item.Credit,
                    Debit = item.Debit,
                    Description = item.Description,
                    CurrencyId = item.CurrencyId
                }, saveInline: false);
        }
        await _cashflowRepository.SaveChangesAsync();
    }

    /// <summary>
    /// money transfer operation between accounts.
    /// </summary>
    /// <param name="amount"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private async Task moneyTransfer(decimal amount, Guid? fromAccount, Guid? toAccount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative.", nameof(amount));

        if ((fromAccount is null || toAccount is null)
            && (fromAccount == Guid.Empty || toAccount == Guid.Empty))
            throw new ArgumentException("FromAccount and ToAccount cannot be empty while for money transfer operation.",
                nameof(fromAccount));

        var cf = _cashflowRepository.TableNoTracking;

        cf = cf.Where(x => x.AccountId == fromAccount);
        if (!cf.Any())
            throw new ArgumentException("FromAccount does not exist or has no cashflows.", nameof(fromAccount));
        
        var actualAmount = amount;

        foreach (var item in cf)
        {
            //TODO : under consideration, this is not the best way to do this.
        }
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
            DocumentNumber = new Random().Next(100000, 999999).ToString()
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
        else
        {
            cashflow.DebitTRY = cashflow.Debit;
            cashflow.CreditTRY = cashflow.Credit;
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

    /// <summary>
    /// get parent child list of all incomes and expenses.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task<IncomeExpenseListDto> GetListOfIncomeExpenseAsync(Guid userId)
    {
        if (await _incomeExpenseRepository.AnyAsync(x => !x.isDeleted))
            throw new Exception("can't find list income/expense. ");

        if (userId == Guid.Empty)
            throw new ArgumentNullException(nameof(userId));

        var userRes = _userService.GetUserDetails(userId);
        if (userRes == null)
            throw new ArgumentNullException(nameof(userRes));
        var user = await userRes;

        var res = _incomeExpenseRepository.JoinWith(
                await _incomeExpenseTypeRepository.FindAsync(x => !x.isDeleted),
                y => y.IncomeExpenseTypeId,
                z => z.Id,
                (y, z) => new IncomeExpenseDto()
                {
                    IncomeExpenseId = y.Id,
                    Icon = y.Icon,
                    ParentId = y.ParentId,
                    Name = y.Name ?? string.Empty,
                    IncomeExpenseTypeDescription = z.Description ?? string.Empty,
                    IncomeExpenseTypeName = z.Name ?? string.Empty,
                }).ToList();

        return new IncomeExpenseListDto()
        {
            UserId = userId,
            Items = res
        };
    }

    /// <summary>
    /// list of all income and expense types.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IncomeExpenseTypeListDto> GetIncomeExpenseTypeListAsync()
    {
        var res = await _incomeExpenseTypeRepository.FindAsync(x => !x.isDeleted);
        if (res == null || !res.Any())
            throw new InvalidOperationException("No income/expense types found.");
        var items = res.Select(x => new IncomeExpenseTypeDto()
        {
            IncomeExpenseTypeId = x.Id,
            Name = x.Name ?? string.Empty,
            Description = x.Description ?? string.Empty,
        });
        return new IncomeExpenseTypeListDto()
        {
            Items = items
        };
    }

    /// <summary>
    /// get list of all cashflow records for a user.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="Exception"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<CashflowListDto> GetCashflowListAsync(GetCashflowListDto model)
    {
        if (model.UserId == Guid.Empty)
            throw new ArgumentNullException(nameof(model.UserId));

        if (model.StartDate.HasValue && model.EndDate.HasValue && model.StartDate > model.EndDate)
            throw new InvalidOperationException("StartDate cannot be greater than EndDate.");

        var userRes = _userService.GetUserDetails(model.UserId);
        if (userRes == null)
            throw new ArgumentNullException(nameof(userRes));
        var user = await userRes;

        if (await _cashflowRepository.AnyAsync(x => x.UserId == user.Id && !x.isDeleted))
            throw new Exception("can't find cashflows for this user");

        var cf = _cashflowRepository.TableNoTracking;
        var cfType = _cashflowTypeRepository.TableNoTracking;
        var currency = _currencyRepository.TableNoTracking;
        var documents = _cashflowDocumentRepository.TableNoTracking;

        if (model.CashflowTypeId.HasValue && model.CashflowTypeId != Guid.Empty)
            cf = cf.Where(x => x.CashflowTypeId == model.CashflowTypeId.Value);

        if (model.StartDate.HasValue && model.EndDate.HasValue)
            cf = cf.Where(x => x.createdAt >= model.StartDate.Value && x.createdAt <= model.EndDate.Value);
        else
        {
            if (model.StartDate.HasValue)
                cf = cf.Where(x => x.createdAt >= model.StartDate.Value);

            if (model.EndDate.HasValue)
                cf = cf.Where(x => x.createdAt <= model.EndDate.Value);
        }

        var res = from item in cf
                  join type in cfType on item.CashflowTypeId equals type.Id
                  join curr in currency on item.CurrencyId equals curr.Id
                  join doc in documents on item.Id equals doc.CashflowId into docGroup
                  from doc in docGroup.DefaultIfEmpty()
                  where item.UserId == model.UserId && !item.isDeleted
                  select new CashflowDto()
                  {
                      CashflowId = item.Id,
                      UserId = item.UserId,
                      CashflowTypeId = item.CashflowTypeId,
                      CashflowTypeName = type.Name ?? string.Empty,
                      Description = item.Description ?? string.Empty,
                      Credit = item.Credit,
                      Debit = item.Debit,
                      CurrencyId = item.CurrencyId,
                      CurrencyRate = item.CurrencyRate,
                      CurrencyCode = curr.CurrencyCode ?? string.Empty,
                      CurrencyName = curr.CurrencyName ?? string.Empty,
                      CreditTRY = item.CreditTRY,
                      DebitTRY = item.DebitTRY,
                      DocumentNumber = doc.DocumentNumber ?? string.Empty,
                      CreatedAt = item.createdAt
                  };

        return new CashflowListDto()
        {
            UserId = model.UserId,
            Items = res
        };
    }
}

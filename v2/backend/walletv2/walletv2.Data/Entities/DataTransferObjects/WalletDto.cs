namespace walletv2.Data.Entities.DataTransferObjects;

public class DistributeCashflowDto
{
    public Guid UserId { get; set; }
    public IEnumerable<DistributeCashflowItemDto>? Items { get; set; }
}

public class DistributeCashflowItemDto
{
    public Guid? FromAccountId { get; set; }
    public Guid? ToAccountId { get; set; }
    public Guid CashflowTypeId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public Guid CurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateAccountDto
{
    public Guid CurrencyId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class CreateIncomeExpenseDto
{
    public Guid UserId { get; set; }
    public Guid IncomeExpenseTypeId { get; set; }
    public Guid? ParentId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class IncomeExpenseListDto
{
    public Guid UserId { get; set; }
    public IEnumerable<IncomeExpenseDto>? Items { get; set; }
}

public class IncomeExpenseDto
{
    public Guid IncomeExpenseId { get; set; }
    public Guid? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IncomeExpenseTypeName { get; set; } = string.Empty;
    public string IncomeExpenseTypeDescription { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}

public class IncomeExpenseTypeListDto
{
    public IEnumerable<IncomeExpenseTypeDto>? Items { get; set; }
}

public class IncomeExpenseTypeDto
{
    public Guid IncomeExpenseTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class CashflowListDto
{
    public Guid UserId { get; set; }
    public IEnumerable<CashflowDto>? Items { get; set; }
}

public class GetCashflowListDto
{
    public Guid UserId { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid? CashflowTypeId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CashflowDto
{
    public Guid CashflowId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CurrencyId { get; set; }
    public Guid CashflowTypeId { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public string CurrencyName { get; set; } = string.Empty;
    public decimal CurrencyRate { get; set; }
    public string? DocumentNumber { get; set; } = string.Empty;
    public string CashflowTypeName { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal DebitTRY { get; set; }
    public decimal CreditTRY { get; set; }
    public DateTime CreatedAt { get; set; }
}
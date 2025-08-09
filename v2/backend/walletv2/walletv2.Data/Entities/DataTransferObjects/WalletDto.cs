namespace walletv2.Data.Entities.DataTransferObjects;

public class DistributeCashflowDto
{
    public Guid UserId { get; set; }
    public IEnumerable<DistributeCashflowItemDto>? Items { get; set; }
}

public class DistributeCashflowItemDto
{
    public Guid CashflowTypeId { get; set; }
    public Guid AccountId { get; set; }
    public decimal Credit { get; set; }
    public decimal Debit { get; set; }
    public Guid CurrencyId { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class  CreateAccountDto
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
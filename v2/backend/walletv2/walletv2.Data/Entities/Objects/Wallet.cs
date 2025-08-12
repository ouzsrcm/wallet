using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities.Objects;

/// <summary>
/// cashflow type entity.
/// </summary>
public class CashflowType : BaseEntityImplementation
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string? Description { get; set; }
}

/// <summary>
/// base cashflow entity.
/// </summary>
public class Cashflow : BaseEntityImplementation
{
    [Required]
    public Guid CashflowTypeId { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public Guid AccountId { get; set; }
    [Required]
    public decimal Credit { get; set; }
    [Required]
    public decimal Debit { get; set; }
    public decimal DebitTRY { get; set; }
    public decimal CreditTRY { get; set; }
    public string? Description { get; set; }
    public Guid? CashflowDocumentId { get; set; }
    public Guid? CurrencyId { get; set; }
    public decimal CurrencyRate { get; set; }
    public virtual User? User { get; set; }
    public virtual Account? Account { get; set; }
    public virtual Currency? Currency { get; set; }
    public virtual CashflowType? CashflowType { get; set; }
    public virtual CashflowDocument? CashflowDocument { get; set; }
}

/// <summary>
/// Account entity representing a user's account.
/// </summary>
public class Account : BaseEntityImplementation
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }
    [Required]
    public Guid CurrencyId { get; set; }
    [Required]
    public Guid UserId { get; set; }
    public virtual User? User { get; set; }
    public virtual Currency? Currency { get; set; }
}

/// <summary>
/// cashflow document entity.
/// </summary>
public class CashflowDocument : BaseEntityImplementation
{
    [Required]
    public Guid CashflowId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
}

/// <summary>
/// Income and Expense Type entity.
/// </summary>
public class IncomeExpenseType : BaseEntityImplementation
{
    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }
    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string? Description { get; set; }
}

/// <summary>
/// income and expense entity.
/// </summary>
public class IncomeExpense : BaseEntityImplementation
{
    public Guid? ParentId { get; set; }

    [Required]
    public Guid IncomeExpenseTypeId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MinLength(3)]
    [MaxLength(100)]
    public string? Name { get; set; }

    [Required]
    [MinLength(10)]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [MaxLength(1000)]
    public string Icon { get; set; } = string.Empty;

    public virtual User? User { get; set; }
}
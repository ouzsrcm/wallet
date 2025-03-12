
using Wallet.Entities.Base.Concrete;
using Wallet.Entities.Enums;

namespace Wallet.Entities.EntityObjects;

public class Transaction : SoftDeleteEntity
{
    public Guid PersonId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = null!;
    public TransactionType Type { get; set; } // Enum: Income/Expense
    public PaymentMethod PaymentMethod { get; set; } // Enum: Cash/Card/BankTransfer etc.
    public string? Reference { get; set; } // Fatura no, sipariş no vs.
    public bool IsRecurring { get; set; }
    public RecurringPeriod? RecurringPeriod { get; set; } // Enum: Daily/Weekly/Monthly/Yearly

    // Navigation Properties
    public Person Person { get; set; } = null!;
    public Category Category { get; set; } = null!;
} 
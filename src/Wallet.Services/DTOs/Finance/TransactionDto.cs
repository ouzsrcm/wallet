using Wallet.Entities.Enums;
namespace Wallet.Services.DTOs.Finance;

public class TransactionDto
{
    public Guid Id { get; set; }
    public Guid PersonId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = null!;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = null!;
    public TransactionType Type { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? Reference { get; set; }
    public bool IsRecurring { get; set; }
    public RecurringPeriod? RecurringPeriod { get; set; }
} 
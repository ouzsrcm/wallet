using Wallet.Entities.Enums;
namespace Wallet.Services.DTOs.Finance;

/// <summary>
/// İşlem detaylarını içeren DTO
/// </summary>
/// <example>
/// {
///   "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
///   "personId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
///   "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
///   "amount": 100.50,
///   "description": "Market alışverişi",
///   "type": 2,
///   "paymentMethod": 1,
///   "reference": "123456",
///   "isRecurring": false,
///   "recurringPeriod": null
/// }
/// </example>
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
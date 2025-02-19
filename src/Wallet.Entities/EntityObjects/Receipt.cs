using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;
public class Receipt : SoftDeleteEntity
{
    public Guid TransactionId { get; set; }
    public string StoreName { get; set; } = null!;
    public string? StoreAddress { get; set; }
    public string? TaxNumber { get; set; }
    public string? ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    public string? PaymentDetails { get; set; }
    public string? Notes { get; set; }
    
    // Navigation Properties
    public Transaction Transaction { get; set; } = null!;
    public ICollection<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
} 
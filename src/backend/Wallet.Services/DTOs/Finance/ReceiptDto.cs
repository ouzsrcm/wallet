namespace Wallet.Services.DTOs.Finance;

public class ReceiptDto
{
    public Guid Id { get; set; }
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
    public List<ReceiptItemDto> Items { get; set; } = new();
} 
using Wallet.Entities.Enums;
namespace Wallet.Services.DTOs.Finance;

public class ReceiptItemDto
{
    public Guid Id { get; set; }
    public Guid ReceiptId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Barcode { get; set; }
    public decimal Quantity { get; set; }
    public UnitType Unit { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
} 
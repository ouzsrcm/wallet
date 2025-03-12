using Wallet.Entities.Base.Concrete;
using Wallet.Entities.Enums;

namespace Wallet.Entities.EntityObjects;

public class ReceiptItem : SoftDeleteEntity
{
    public Guid ReceiptId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? Barcode { get; set; }
    public decimal Quantity { get; set; }
    public UnitType Unit { get; set; } // string yerine enum kullanıyoruz
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? TaxAmount { get; set; }
    public decimal? DiscountAmount { get; set; }
    
    // Navigation Properties
    public Receipt Receipt { get; set; } = null!;
} 
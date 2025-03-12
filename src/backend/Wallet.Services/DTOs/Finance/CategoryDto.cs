using Wallet.Entities.Enums;
namespace Wallet.Services.DTOs.Finance;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string Color { get; set; } = null!;
    public TransactionType Type { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public bool IsSystem { get; set; }
} 
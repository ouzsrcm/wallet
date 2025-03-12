using Wallet.Entities.Base.Concrete;
using Wallet.Entities.Enums;

namespace Wallet.Entities.EntityObjects
{
    public class Category : SoftDeleteEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string Color { get; set; } = null!;
        public TransactionType Type { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public bool IsSystem { get; set; } // Sistem tarafından oluşturulan kategoriler
        
        // Navigation Properties
        public Category? ParentCategory { get; set; }
        public ICollection<Category>? SubCategories { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
    }
} 
using Wallet.Entities.Base.Abstract;

namespace Wallet.Entities.Base.Concrete;

public abstract class SoftDeleteEntity : AuditableEntity, ISoftDeleteEntity
{
    public DateTime? DeletedAt { get; set; }
    public string? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public string? DeletedByIp { get; set; }
    public bool IsDeleted { get; set; }
} 
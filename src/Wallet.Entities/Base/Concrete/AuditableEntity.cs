using Wallet.Entities.Base.Abstract;

namespace Wallet.Entities.Base.Concrete;

public abstract class AuditableEntity : BaseEntity, ICreatedEntity, IModifiedEntity
{
    public DateTime CreatedDate { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedByIp { get; set; }
} 
using Wallet.Entities.Base.Abstract;

namespace Wallet.Entities.Base.Concrete;

public abstract class BaseEntity : IEntity, IAuditEntity
{
    public Guid Id { get; set; }
    public int RowVersion { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Location { get; set; }
    
} 
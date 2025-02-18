using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class MessageAttachment : SoftDeleteEntity
{
    public Guid MessageId { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = null!;
    
    // Navigation property
    public virtual Message Message { get; set; } = null!;
} 
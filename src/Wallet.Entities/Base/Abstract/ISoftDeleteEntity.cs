namespace Wallet.Entities.Base.Abstract;

public interface ISoftDeleteEntity
{
    
    // Deletion info
    public DateTime? DeletedAt { get; set; }
    public string? DeletedByUserId { get; set; }
    public string? DeletedByUserName { get; set; }
    public string? DeletedByIp { get; set; }
    public bool IsDeleted { get; set; }
} 
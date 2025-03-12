namespace Wallet.Entities.Base.Abstract;

public interface IModifiedEntity
{
    public DateTime? ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    public string? ModifiedByIp { get; set; }
} 
namespace Wallet.Entities.Base.Abstract;

public interface ICreatedEntity
{
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
} 
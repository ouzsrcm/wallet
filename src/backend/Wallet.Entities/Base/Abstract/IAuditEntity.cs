namespace Wallet.Entities.Base.Abstract;

public interface IAuditEntity
{
    public int RowVersion { get; set; }
    public string IPAddress { get; set; }
    public string UserAgent { get; set; }
    public string Location { get; set; }
    
} 
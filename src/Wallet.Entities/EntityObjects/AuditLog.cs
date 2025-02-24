using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class AuditLog : SoftDeleteEntity
{
    public string? EntityName { get; set; }
    public string? ActionType { get; set; } // Create, Update, Delete, SoftDelete
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime ActionDate { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public string? TableName { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestBody { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? AdditionalInfo { get; set; }
} 
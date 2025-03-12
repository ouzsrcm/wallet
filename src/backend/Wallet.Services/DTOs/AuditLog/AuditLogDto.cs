namespace Wallet.Services.DTOs.AuditLog;

public class AuditLogDto
{
    public Guid Id { get; set; }
    public string EntityName { get; set; } = null!;
    public string ActionType { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public DateTime ActionDate { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public string? TableName { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestBody { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? StackTrace { get; set; }
    public string? AdditionalInfo { get; set; }
}

public class CreateAuditLogDto
{
    public required string EntityName { get; set; }
    public required string ActionType { get; set; }
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public string? AffectedColumns { get; set; }
    public string? PrimaryKey { get; set; }
    public string? TableName { get; set; }
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? RequestUrl { get; set; }
    public string? RequestMethod { get; set; }
    public string? RequestBody { get; set; }
    public int? ResponseStatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public string? AdditionalInfo { get; set; }
}

public class AuditLogFilterDto
{
    public string? EntityName { get; set; }
    public string? ActionType { get; set; }
    public string? UserId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class AuditLogSummaryDto
{
    public int TotalLogs { get; set; }
    public int CreateCount { get; set; }
    public int UpdateCount { get; set; }
    public int DeleteCount { get; set; }
    public int ErrorCount { get; set; }
    public Dictionary<string, int> ActionsByEntity { get; set; } = new();
    public Dictionary<string, int> ActionsByUser { get; set; } = new();
} 
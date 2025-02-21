using Wallet.Services.DTOs.AuditLog;

namespace Wallet.Services.Abstract;

public interface IAuditLogService
{
    Task<AuditLogDto> GetByIdAsync(Guid id);
    Task<List<AuditLogDto>> GetAllAsync(AuditLogFilterDto filter);
    Task<List<AuditLogDto>> GetByUserIdAsync(string userId);
    Task<List<AuditLogDto>> GetByEntityNameAsync(string entityName);
    Task<List<AuditLogDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<List<AuditLogDto>> GetByActionTypeAsync(string actionType);
    Task<AuditLogDto> CreateAsync(CreateAuditLogDto auditLogDto);
    Task<List<string>> GetDistinctEntityNamesAsync();
    Task<List<string>> GetDistinctActionTypesAsync();
    Task<List<string>> GetDistinctUsersAsync();
    Task DeleteOlderThanAsync(DateTime date);
    Task<AuditLogSummaryDto> GetAuditLogSummaryAsync(DateTime startDate, DateTime endDate);
} 
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Wallet.Entities.EntityObjects;
using Wallet.Entities.Base.Concrete;
using Wallet.Infrastructure.Abstract;
using Wallet.Infrastructure.Services;

namespace Wallet.DataLayer.Interceptors;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService;

    public AuditSaveChangesInterceptor(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var auditLogs = await CreateAuditLogs(context);
        
        // AuditLog'ları context'e ekle
        foreach (var auditLog in auditLogs)
        {
            context.Set<AuditLog>().Add(auditLog);
        }

        return result;
    }

    private async Task<List<AuditLog>> CreateAuditLogs(DbContext context)
    {
        var auditLogs = new List<AuditLog>();
        var entries = context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        foreach (var entry in entries)
        {
            var auditType = "Create";
            switch(entry.State)
            {
                case EntityState.Added:
                    auditType = "Create";
                    break;
                case EntityState.Modified:
                    auditType = "Update";
                    break;
                case EntityState.Deleted:
                    auditType = "Delete";
                    break;
            }

            var auditLog = new AuditLog()
            {
                ActionType = auditType,
                EntityName = entry.Entity.GetType().Name,
                UserId = _currentUserService.GetCurrentUserId() ?? "system",
                UserName = _currentUserService.GetCurrentUserName() ?? "System",
                ActionDate = DateTime.UtcNow,
                TableName = context.Model.FindEntityType(entry.Entity.GetType())?.GetTableName(),
                IPAddress = _currentUserService.GetIpAddress(),
                UserAgent = _currentUserService.GetUserAgent(),
                RequestUrl = _currentUserService.GetRequestUrl(),
                RequestMethod = _currentUserService.GetRequestMethod(),
                CreatedBy = _currentUserService.GetCurrentUserId() ?? "system",
                CreatedDate = DateTime.UtcNow
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditLog.NewValues = SerializeEntity(entry);
                    break;
                case EntityState.Modified:
                    auditLog.OldValues = SerializeOriginalValues(entry);
                    auditLog.NewValues = SerializeCurrentValues(entry);
                    auditLog.AffectedColumns = string.Join(", ", entry.Properties
                        .Where(p => p.IsModified)
                        .Select(p => p.Metadata.Name));
                    break;
                case EntityState.Deleted:
                    auditLog.OldValues = SerializeEntity(entry);
                    break;
            }
            // Primary key değerini al
            var keyValues = entry.Properties
                .Where(p => p.Metadata.IsPrimaryKey())
                .Select(p => p.CurrentValue)
                .ToList();
            auditLog.PrimaryKey = string.Join(",", keyValues);

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    private string? SerializeEntity(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        
        foreach (var property in entry.Properties)
        {
            // Hassas verileri loglama
            if (property.Metadata.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                property.Metadata.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase))
                continue;

            values[property.Metadata.Name] = property.CurrentValue;
        }

        return JsonSerializer.Serialize(values);
    }

    private string? SerializeOriginalValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        
        foreach (var property in entry.Properties)
        {
            if (!property.IsModified) continue;
            
            // Hassas verileri loglama
            if (property.Metadata.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                property.Metadata.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase))
                continue;

            values[property.Metadata.Name] = property.OriginalValue;
        }

        return JsonSerializer.Serialize(values);
    }

    private string? SerializeCurrentValues(EntityEntry entry)
    {
        var values = new Dictionary<string, object?>();
        
        foreach (var property in entry.Properties)
        {
            if (!property.IsModified) continue;
            
            // Hassas verileri loglama
            if (property.Metadata.Name.Contains("Password", StringComparison.OrdinalIgnoreCase) ||
                property.Metadata.Name.Contains("Secret", StringComparison.OrdinalIgnoreCase))
                continue;

            values[property.Metadata.Name] = property.CurrentValue;
        }

        return JsonSerializer.Serialize(values);
    }
} 
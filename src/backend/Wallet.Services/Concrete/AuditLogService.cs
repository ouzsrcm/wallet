using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.AuditLog;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Entities.EntityObjects;

namespace Wallet.Services.Concrete;

public class AuditLogService : IAuditLogService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AuditLogService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<AuditLogDto> GetByIdAsync(Guid id)
    {
        var auditLog = await _unitOfWork.GetRepository<AuditLog>()
            .GetByIdAsync(id);
        return _mapper.Map<AuditLogDto>(auditLog);
    }

    public async Task<List<AuditLogDto>> GetAllAsync(AuditLogFilterDto filter)
    {
        var query = _unitOfWork.GetRepository<AuditLog>().GetAll()
            .AsNoTracking();

        // Filtreleri uygula
        if (!string.IsNullOrEmpty(filter.EntityName))
            query = query.Where(x => x.EntityName == filter.EntityName);

        if (!string.IsNullOrEmpty(filter.ActionType))
            query = query.Where(x => x.ActionType == filter.ActionType);

        if (!string.IsNullOrEmpty(filter.UserId))
            query = query.Where(x => x.UserId == filter.UserId);

        if (filter.StartDate.HasValue)
            query = query.Where(x => x.ActionDate >= filter.StartDate);

        if (filter.EndDate.HasValue)
            query = query.Where(x => x.ActionDate <= filter.EndDate);

        if (!string.IsNullOrEmpty(filter.SearchTerm))
        {
            query = query.Where(x =>
                x.EntityName.Contains(filter.SearchTerm) ||
                x.UserName.Contains(filter.SearchTerm) ||
                x.TableName!.Contains(filter.SearchTerm) ||
                x.RequestUrl!.Contains(filter.SearchTerm)
            );
        }

        // Sayfalama
        query = query.OrderByDescending(x => x.ActionDate)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize);

        var auditLogs = await query.ToListAsync();
        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }

    public async Task<List<AuditLogDto>> GetByUserIdAsync(string userId)
    {
        var auditLogs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.UserId == userId)
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync();
        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }

    public async Task<List<AuditLogDto>> GetByEntityNameAsync(string entityName)
    {
        var auditLogs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.EntityName == entityName)
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync();
        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }

    public async Task<List<AuditLogDto>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        var auditLogs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.ActionDate >= startDate && x.ActionDate <= endDate)
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync();
        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }

    public async Task<List<AuditLogDto>> GetByActionTypeAsync(string actionType)
    {
        var auditLogs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.ActionType == actionType)
            .OrderByDescending(x => x.ActionDate)
            .ToListAsync();
        return _mapper.Map<List<AuditLogDto>>(auditLogs);
    }

    public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto auditLogDto)
    {
        var auditLog = _mapper.Map<AuditLog>(auditLogDto);
        auditLog = await _unitOfWork.GetRepository<AuditLog>().AddAsync(auditLog);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<AuditLogDto>(auditLog);
    }

    public async Task<List<string>> GetDistinctEntityNamesAsync()
    {
        return await _unitOfWork.GetRepository<AuditLog>()
            .GetAll()
            .Select(x => x.EntityName)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<string>> GetDistinctActionTypesAsync()
    {
        return await _unitOfWork.GetRepository<AuditLog>()
            .GetAll()
            .Select(x => x.ActionType)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<string>> GetDistinctUsersAsync()
    {
        return await _unitOfWork.GetRepository<AuditLog>()
            .GetAll()
            .Select(x => x.UserName)
            .Distinct()
            .ToListAsync();
    }

    public async Task DeleteOlderThanAsync(DateTime date)
    {
        var oldLogs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.ActionDate < date)
            .ToListAsync();

        await _unitOfWork.GetRepository<AuditLog>().RemoveRangeAsync(oldLogs);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<AuditLogSummaryDto> GetAuditLogSummaryAsync(DateTime startDate, DateTime endDate)
    {
        var logs = await _unitOfWork.GetRepository<AuditLog>()
            .GetWhere(x => x.ActionDate >= startDate && x.ActionDate <= endDate)
            .ToListAsync();

        var summary = new AuditLogSummaryDto
        {
            TotalLogs = logs.Count,
            CreateCount = logs.Count(x => x.ActionType == "Create"),
            UpdateCount = logs.Count(x => x.ActionType == "Update"),
            DeleteCount = logs.Count(x => x.ActionType == "Delete"),
            ErrorCount = logs.Count(x => !string.IsNullOrEmpty(x.ErrorMessage)),
            ActionsByEntity = logs.GroupBy(x => x.EntityName)
                                .ToDictionary(g => g.Key, g => g.Count()),
            ActionsByUser = logs.GroupBy(x => x.UserName)
                               .ToDictionary(g => g.Key, g => g.Count())
        };

        return summary;
    }
} 
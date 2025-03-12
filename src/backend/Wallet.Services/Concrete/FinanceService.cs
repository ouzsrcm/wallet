using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Wallet.Services.Exceptions;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Entities.EntityObjects;
using Wallet.Entities.Enums;
using Wallet.Infrastructure.Abstract;
namespace Wallet.Services.Concrete;

public class FinanceService : IFinanceService
{
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public FinanceService(IFinanceUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUserService = currentUserService;
    }

    public async Task<Guid> PersonId()
    {
        var userId = _currentUserService.GetCurrentUserId() 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        if (!Guid.TryParse(userId, out var userIdGuid))
            throw new BadRequestException("Invalid user ID");

        var user = await _unitOfWork.GetRepository<User>().GetByIdAsync(userIdGuid)
            ?? throw new NotFoundException("User not found");

        return user.PersonId;
    }

    // Category operations
    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>()
            .GetAllAsync(
                predicate: c => c.Id == id && !c.IsDeleted,
                include: null,
                selector: c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Type = c.Type,
                    Icon = c.Icon,
                    Color = c.Color,
                    ParentCategoryId = c.ParentCategoryId,
                    IsSystem = c.IsSystem
                });

        return category.FirstOrDefault() ?? throw new NotFoundException("Category not found");
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(bool includeDeleted = false)
    {
        var query = await _unitOfWork.GetRepository<Category>()
                        .GetWhere(c => !c.IsDeleted)
                        .Select(c => new CategoryDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Description = c.Description,
                            Type = c.Type,
                            Icon = c.Icon,
                            Color = c.Color,
                            ParentCategoryId = c.ParentCategoryId,
                            IsSystem = c.IsSystem
                        }).ToListAsync();

        return query;
    }

    public async Task<List<CategoryDto>> GetCategoriesByTypeAsync(TransactionType type)
    {
        var categories = await _unitOfWork.GetRepository<Category>()
            .GetAllAsync(
                predicate: c => c.Type == type && !c.IsDeleted,
                include: null,
                selector: c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Type = c.Type,
                    Icon = c.Icon,
                    Color = c.Color,
                    ParentCategoryId = c.ParentCategoryId,
                    IsSystem = c.IsSystem
                });

        return categories;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _unitOfWork.GetRepository<Category>().AddAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto categoryDto)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        category.Name = categoryDto.Name;
        category.Description = categoryDto.Description;
        category.Type = categoryDto.Type;
        category.Icon = categoryDto.Icon;
        category.Color = categoryDto.Color;
        category.ParentCategoryId = categoryDto.ParentCategoryId;

        await _unitOfWork.GetRepository<Category>().UpdateAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        await _unitOfWork.GetRepository<Category>().SoftDeleteAsync(category);
    }

    // Transaction operations
    public async Task<TransactionDto> GetTransactionByIdAsync(Guid id)
    {
        var personId = await PersonId();

        var transaction = await _unitOfWork.GetRepository<Transaction>()
            .GetAllAsync<TransactionDto>(
                predicate: t => t.Id == id && t.PersonId == personId && !t.IsDeleted,
                include: q => q
                    .Include(t => t.Category),
                selector: t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Currency = t.Currency,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Type = t.Type,
                    PaymentMethod = t.PaymentMethod,
                    Reference = t.Reference,
                    IsRecurring = t.IsRecurring,
                    RecurringPeriod = t.RecurringPeriod,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name
                });

        if (transaction == null || !transaction.Any())
            throw new NotFoundException("Transaction not found");

        return transaction.FirstOrDefault() ?? throw new NotFoundException("Transaction not found");
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync()
    {
        var personId = await PersonId();

        var transactions = await _unitOfWork.GetRepository<Transaction>()
            .GetAllAsync<TransactionDto>(
                predicate: t => t.PersonId == personId && !t.IsDeleted,
                include: q => q
                    .Include(t => t.Category),
                selector: t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Currency = t.Currency,
                    TransactionDate = t.TransactionDate,
                    Description = t.Description,
                    Type = t.Type,
                    PaymentMethod = t.PaymentMethod,
                    Reference = t.Reference,
                    IsRecurring = t.IsRecurring,
                    RecurringPeriod = t.RecurringPeriod,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category.Name
                });
        return transactions;
    }

    public async Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = new Transaction()
        {
            Amount = transactionDto.Amount,
            Currency = transactionDto.Currency ?? "TRY",
            TransactionDate = transactionDto.TransactionDate,
            Description = transactionDto.Description,
            Type = transactionDto.Type,
            PaymentMethod = transactionDto.PaymentMethod,
            Reference = transactionDto.Reference,
            IsRecurring = transactionDto.IsRecurring,
            RecurringPeriod = transactionDto.RecurringPeriod,
            CategoryId = transactionDto.CategoryId,
            PersonId = await PersonId()
        };

        await _unitOfWork.GetRepository<Transaction>().AddAsync(transaction);

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid id, TransactionDto transactionDto)
    {
        var transaction = await _unitOfWork.GetRepository<Transaction>().GetByIdAsync(id)
            ?? throw new NotFoundException("Transaction not found");

        _mapper.Map(transactionDto, transaction);
        await _unitOfWork.GetRepository<Transaction>().UpdateAsync(transaction);
        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var transaction = await _unitOfWork.GetRepository<Transaction>().GetByIdAsync(id)
            ?? throw new NotFoundException("Transaction not found");

        await _unitOfWork.GetRepository<Transaction>().SoftDeleteAsync(transaction);
    }

    // Receipt operations
    public async Task<ReceiptDto> GetReceiptByIdAsync(Guid id)
    {
        var receipt = await _unitOfWork.GetRepository<Receipt>()
            .GetWhere(r => r.Id == id && !r.IsDeleted)
            .Include(r => r.Items.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync();

        if (receipt == null)
            throw new NotFoundException("Receipt not found");

        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task<List<ReceiptDto>> GetReceiptsByTransactionIdAsync(Guid transactionId)
    {
        var receipts = await _unitOfWork.GetRepository<Receipt>()
            .GetWhere(r => r.TransactionId == transactionId && !r.IsDeleted)
            .Include(r => r.Items.Where(i => !i.IsDeleted))
            .ToListAsync();

        return _mapper.Map<List<ReceiptDto>>(receipts);
    }

    public async Task<ReceiptDto> CreateReceiptAsync(ReceiptDto receiptDto)
    {
        var receipt = _mapper.Map<Receipt>(receiptDto);
        await _unitOfWork.GetRepository<Receipt>().AddAsync(receipt);
        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task<ReceiptDto> UpdateReceiptAsync(Guid id, ReceiptDto receiptDto)
    {
        var receipt = await _unitOfWork.GetRepository<Receipt>().GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt not found");

        _mapper.Map(receiptDto, receipt);
        await _unitOfWork.GetRepository<Receipt>().UpdateAsync(receipt);
        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task DeleteReceiptAsync(Guid id)
    {
        var receipt = await _unitOfWork.GetRepository<Receipt>().GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt not found");

        await _unitOfWork.GetRepository<Receipt>().SoftDeleteAsync(receipt);
    }

    // ReceiptItem operations
    public async Task<ReceiptItemDto> GetReceiptItemByIdAsync(Guid id)
    {
        var receiptItem = await _unitOfWork.GetRepository<ReceiptItem>()
            .GetWhere(ri => ri.Id == id && !ri.IsDeleted)
            .FirstOrDefaultAsync();

        if (receiptItem == null)
            throw new NotFoundException("Receipt item not found");

        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task<List<ReceiptItemDto>> GetReceiptItemsByReceiptIdAsync(Guid receiptId)
    {
        var receiptItems = await _unitOfWork.GetRepository<ReceiptItem>()
            .GetWhere(ri => ri.ReceiptId == receiptId && !ri.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<ReceiptItemDto>>(receiptItems);
    }

    public async Task<ReceiptItemDto> CreateReceiptItemAsync(ReceiptItemDto receiptItemDto)
    {
        var receiptItem = _mapper.Map<ReceiptItem>(receiptItemDto);
        await _unitOfWork.GetRepository<ReceiptItem>().AddAsync(receiptItem);
        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task<ReceiptItemDto> UpdateReceiptItemAsync(Guid id, ReceiptItemDto receiptItemDto)
    {
        var receiptItem = await _unitOfWork.GetRepository<ReceiptItem>().GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt item not found");

        _mapper.Map(receiptItemDto, receiptItem);
        await _unitOfWork.GetRepository<ReceiptItem>().UpdateAsync(receiptItem);
        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task DeleteReceiptItemAsync(Guid id)
    {
        var receiptItem = await _unitOfWork.GetRepository<ReceiptItem>().GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt item not found");

        await _unitOfWork.GetRepository<ReceiptItem>().SoftDeleteAsync(receiptItem);
    }

    public async Task<List<ReceiptDto>> GetAllReceiptsAsync(Guid userId)
    {
        var receipts = await _unitOfWork.GetRepository<Receipt>()
            .GetAllAsync<ReceiptDto>(
                predicate: r => r.Transaction.PersonId == userId && !r.IsDeleted,
                include: q => q
                    .Include(r => r.Transaction)
                    .Include(r => r.Items),
                    selector: r => new ReceiptDto
                    {
                        Id = r.Id,
                        TransactionId = r.TransactionId,
                        Items = r.Items.Select(i => new ReceiptItemDto
                        {
                            Id = i.Id,
                            ReceiptId = i.ReceiptId,
                            Quantity = i.Quantity,
                            UnitPrice = i.UnitPrice,
                            TotalPrice = i.TotalPrice,
                            ProductName = i.ProductName,
                            Barcode = i.Barcode,
                            Unit = i.Unit,
                            TaxRate = i.TaxRate,
                            TaxAmount = i.TaxAmount
                        }).ToList()
                    }
            );

        if (receipts == null)
            throw new NotFoundException($"User {userId} has no receipts");

        return receipts;
    }

    public async Task<List<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _unitOfWork.GetRepository<Category>()
            .GetAllAsync<CategoryDto>(
                predicate: c => !c.IsDeleted,
                include: q => q
                    .Include(c => c.Transactions.Where(t => !t.IsDeleted)),
                    selector: c => new CategoryDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Description = c.Description,
                        Type = c.Type,
                        Icon = c.Icon,
                        Color = c.Color,
                        ParentCategoryId = c.ParentCategoryId,
                        IsSystem = c.IsSystem
                    }
            );

        if (categories == null || !categories.Any())
            throw new NotFoundException("No categories found");

        return categories;
    }
}
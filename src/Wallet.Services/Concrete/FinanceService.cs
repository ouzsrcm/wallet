using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Wallet.Services.Exceptions;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Entities.EntityObjects;
using Wallet.Entities.Enums;

namespace Wallet.Services.Concrete;

public class FinanceService : IFinanceService
{
    private readonly IFinanceUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public FinanceService(IFinanceUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    // Category operations
    public async Task<CategoryDto> GetCategoryByIdAsync(Guid id)
    {
        var category = await _unitOfWork.Categories
            .GetWhere(c => c.Id == id && !c.IsDeleted)
            .FirstOrDefaultAsync();

        if (category == null)
            throw new NotFoundException("Category not found");

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<List<CategoryDto>> GetCategoriesAsync(bool includeDeleted = false)
    {
        var query = _unitOfWork.Categories.GetAll();
        if (!includeDeleted)
            query = query.Where(c => !c.IsDeleted);

        var categories = await query.ToListAsync();
        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<List<CategoryDto>> GetCategoriesByTypeAsync(TransactionType type)
    {
        var categories = await _unitOfWork.Categories
            .GetWhere(c => c.Type == type && !c.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _unitOfWork.Categories.AddAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto categoryDto)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        _mapper.Map(categoryDto, category);
        await _unitOfWork.Categories.UpdateAsync(category);
        return _mapper.Map<CategoryDto>(category);
    }

    public async Task DeleteCategoryAsync(Guid id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id)
            ?? throw new NotFoundException("Category not found");

        await _unitOfWork.Categories.SoftDeleteAsync(category);
    }

    // Transaction operations
    public async Task<TransactionDto> GetTransactionByIdAsync(Guid id)
    {
        var transaction = await _unitOfWork.Transactions
            .GetWhere(t => t.Id == id && !t.IsDeleted)
            .FirstOrDefaultAsync();

        if (transaction == null)
            throw new NotFoundException("Transaction not found");

        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<List<TransactionDto>> GetTransactionsAsync(Guid personId)
    {
        var transactions = await _unitOfWork.Transactions
            .GetWhere(t => t.PersonId == personId && !t.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<TransactionDto>>(transactions);
    }

    public async Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto)
    {
        var transaction = _mapper.Map<Transaction>(transactionDto);
        await _unitOfWork.Transactions.AddAsync(transaction);
        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task<TransactionDto> UpdateTransactionAsync(Guid id, TransactionDto transactionDto)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(id)
            ?? throw new NotFoundException("Transaction not found");

        _mapper.Map(transactionDto, transaction);
        await _unitOfWork.Transactions.UpdateAsync(transaction);
        return _mapper.Map<TransactionDto>(transaction);
    }

    public async Task DeleteTransactionAsync(Guid id)
    {
        var transaction = await _unitOfWork.Transactions.GetByIdAsync(id)
            ?? throw new NotFoundException("Transaction not found");

        await _unitOfWork.Transactions.SoftDeleteAsync(transaction);
    }

    // Receipt operations
    public async Task<ReceiptDto> GetReceiptByIdAsync(Guid id)
    {
        var receipt = await _unitOfWork.Receipts
            .GetWhere(r => r.Id == id && !r.IsDeleted)
            .Include(r => r.Items.Where(i => !i.IsDeleted))
            .FirstOrDefaultAsync();

        if (receipt == null)
            throw new NotFoundException("Receipt not found");

        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task<List<ReceiptDto>> GetReceiptsByTransactionIdAsync(Guid transactionId)
    {
        var receipts = await _unitOfWork.Receipts
            .GetWhere(r => r.TransactionId == transactionId && !r.IsDeleted)
            .Include(r => r.Items.Where(i => !i.IsDeleted))
            .ToListAsync();

        return _mapper.Map<List<ReceiptDto>>(receipts);
    }

    public async Task<ReceiptDto> CreateReceiptAsync(ReceiptDto receiptDto)
    {
        var receipt = _mapper.Map<Receipt>(receiptDto);
        await _unitOfWork.Receipts.AddAsync(receipt);
        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task<ReceiptDto> UpdateReceiptAsync(Guid id, ReceiptDto receiptDto)
    {
        var receipt = await _unitOfWork.Receipts.GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt not found");

        _mapper.Map(receiptDto, receipt);
        await _unitOfWork.Receipts.UpdateAsync(receipt);
        return _mapper.Map<ReceiptDto>(receipt);
    }

    public async Task DeleteReceiptAsync(Guid id)
    {
        var receipt = await _unitOfWork.Receipts.GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt not found");

        await _unitOfWork.Receipts.SoftDeleteAsync(receipt);
    }

    // ReceiptItem operations
    public async Task<ReceiptItemDto> GetReceiptItemByIdAsync(Guid id)
    {
        var receiptItem = await _unitOfWork.ReceiptItems
            .GetWhere(ri => ri.Id == id && !ri.IsDeleted)
            .FirstOrDefaultAsync();

        if (receiptItem == null)
            throw new NotFoundException("Receipt item not found");

        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task<List<ReceiptItemDto>> GetReceiptItemsByReceiptIdAsync(Guid receiptId)
    {
        var receiptItems = await _unitOfWork.ReceiptItems
            .GetWhere(ri => ri.ReceiptId == receiptId && !ri.IsDeleted)
            .ToListAsync();

        return _mapper.Map<List<ReceiptItemDto>>(receiptItems);
    }

    public async Task<ReceiptItemDto> CreateReceiptItemAsync(ReceiptItemDto receiptItemDto)
    {
        var receiptItem = _mapper.Map<ReceiptItem>(receiptItemDto);
        await _unitOfWork.ReceiptItems.AddAsync(receiptItem);
        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task<ReceiptItemDto> UpdateReceiptItemAsync(Guid id, ReceiptItemDto receiptItemDto)
    {
        var receiptItem = await _unitOfWork.ReceiptItems.GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt item not found");

        _mapper.Map(receiptItemDto, receiptItem);
        await _unitOfWork.ReceiptItems.UpdateAsync(receiptItem);
        return _mapper.Map<ReceiptItemDto>(receiptItem);
    }

    public async Task DeleteReceiptItemAsync(Guid id)
    {
        var receiptItem = await _unitOfWork.ReceiptItems.GetByIdAsync(id)
            ?? throw new NotFoundException("Receipt item not found");

        await _unitOfWork.ReceiptItems.SoftDeleteAsync(receiptItem);
    }
} 
using Wallet.Entities.Enums;
using Wallet.Services.DTOs.Finance;

namespace Wallet.Services.Abstract;

public interface IFinanceService
{
    // Category operations
    Task<CategoryDto> GetCategoryByIdAsync(Guid id);
    Task<List<CategoryDto>> GetCategoriesAsync(bool includeDeleted = false);
    Task<List<CategoryDto>> GetCategoriesByTypeAsync(TransactionType type);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryDto> UpdateCategoryAsync(Guid id, CategoryDto categoryDto);
    Task DeleteCategoryAsync(Guid id);

    // Transaction operations
    Task<TransactionDto> GetTransactionByIdAsync(Guid id);
    Task<List<TransactionDto>> GetTransactionsAsync(Guid personId);
    Task<TransactionDto> CreateTransactionAsync(TransactionDto transactionDto);
    Task<TransactionDto> UpdateTransactionAsync(Guid id, TransactionDto transactionDto);
    Task DeleteTransactionAsync(Guid id);

    // Receipt operations
    Task<ReceiptDto> GetReceiptByIdAsync(Guid id);
    Task<List<ReceiptDto>> GetReceiptsByTransactionIdAsync(Guid transactionId);
    Task<ReceiptDto> CreateReceiptAsync(ReceiptDto receiptDto);
    Task<ReceiptDto> UpdateReceiptAsync(Guid id, ReceiptDto receiptDto);
    Task DeleteReceiptAsync(Guid id);

    // ReceiptItem operations
    Task<ReceiptItemDto> GetReceiptItemByIdAsync(Guid id);
    Task<List<ReceiptItemDto>> GetReceiptItemsByReceiptIdAsync(Guid receiptId);
    Task<ReceiptItemDto> CreateReceiptItemAsync(ReceiptItemDto receiptItemDto);
    Task<ReceiptItemDto> UpdateReceiptItemAsync(Guid id, ReceiptItemDto receiptItemDto);
    Task DeleteReceiptItemAsync(Guid id);
} 
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Wallet.Services.Exceptions;
using Wallet.Infrastructure.Abstract;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using System;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[Tags("Transactions")]
public class TransactionsController : ControllerBase
{
    private readonly IFinanceService _financeService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TransactionsController> _logger;

    private Guid userId
    {
        get{
            var res = _currentUserService.GetCurrentUserId();
            _logger.LogInformation("User ID: {UserId}", res);
            if (res == null)
            {
                _logger.LogWarning("User ID not found in token");
                return Guid.Empty;
            }
            return Guid.TryParse(res, out var userIdGuid) ? userIdGuid : Guid.Empty;
        }   
    }

    public TransactionsController(
        IFinanceService financeService,
        ICurrentUserService currentUserService,
        ILogger<TransactionsController> logger)
    {
        _logger = logger;
        _financeService = financeService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Tüm işlemleri listeler
    /// </summary>
    /// <returns>İşlem listesi</returns>
    /// <response code="200">İşlemler başarıyla getirildi</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {        
        try
        {
            _logger.LogInformation("Getting all transactions for user {UserId}", userId);

            var transactions = await _financeService.GetTransactionsAsync();

            _logger.LogInformation("Retrieved {Count} transactions for user {UserId}",
                transactions.Count, userId);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transactions for user {UserId}", userId);
            return StatusCode(500, new { message = "İşlemler getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// ID'ye göre işlem getirir
    /// </summary>
    /// <param name="id">İşlem ID</param>
    /// <returns>İşlem detayı</returns>
    /// <response code="200">İşlem bulundu</response>
    /// <response code="404">İşlem bulunamadı</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting transaction {TransactionId}", id);

            var transaction = await _financeService.GetTransactionByIdAsync(id);
            if (transaction == null)
            {
                _logger.LogWarning("Transaction {TransactionId} not found", id);
                return NotFound();
            }

            _logger.LogInformation("Retrieved transaction {TransactionId}", id);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting transaction {TransactionId}", id);
            return StatusCode(500, new { message = "İşlem getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Yeni işlem oluşturur
    /// </summary>
    /// <param name="transactionDto">İşlem bilgileri</param>
    /// <returns>Oluşturulan işlem</returns>
    /// <response code="201">İşlem başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto)
    {
        try
        {
            _logger.LogInformation("Creating new transaction for user {UserId}", userId);

            var created = await _financeService.CreateTransactionAsync(transactionDto);

            _logger.LogInformation("Created transaction {TransactionId} for user {UserId}",
                created.Id, userId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create transaction: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating transaction for user {UserId}",
                userId);
            return StatusCode(500, new { message = "İşlem oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// İşlem günceller
    /// </summary>
    /// <param name="id">İşlem ID</param>
    /// <param name="transactionDto">Güncellenecek işlem bilgileri</param>
    /// <returns>Güncellenen işlem</returns>
    /// <response code="200">İşlem başarıyla güncellendi</response>
    /// <response code="404">İşlem bulunamadı</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TransactionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TransactionDto transactionDto)
    {
        try
        {
            _logger.LogInformation("Updating transaction {TransactionId}", id);

            var updated = await _financeService.UpdateTransactionAsync(id, transactionDto);

            _logger.LogInformation("Updated transaction {TransactionId}", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Transaction {TransactionId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating transaction {TransactionId}", id);
            return StatusCode(500, new { message = "İşlem güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// İşlem siler
    /// </summary>
    /// <param name="id">İşlem ID</param>
    /// <returns>Silme durumu</returns>
    /// <response code="204">İşlem başarıyla silindi</response>
    /// <response code="404">İşlem bulunamadı</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting transaction {TransactionId}", id);

            await _financeService.DeleteTransactionAsync(id);

            _logger.LogInformation("Deleted transaction {TransactionId}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Transaction {TransactionId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting transaction {TransactionId}", id);
            return StatusCode(500, new { message = "İşlem silinirken bir hata oluştu" });
        }
    }
}
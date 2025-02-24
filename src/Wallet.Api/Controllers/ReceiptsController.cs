using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;
using Microsoft.Extensions.Logging;
using Wallet.Services.Exceptions;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IFinanceService _financeService;
    private readonly ILogger<ReceiptsController> _logger;

    public ReceiptsController(
        IFinanceService financeService,
        ILogger<ReceiptsController> logger)
    {
        _financeService = financeService;
        _logger = logger;
    }

    /// <summary>
    /// Tüm fişleri listeler
    /// </summary>
    /// <returns>Fiş listesi</returns>
    /// <response code="200">Fişler başarıyla getirildi</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ReceiptDto>), 200)]
    public async Task<IActionResult> GetAll(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting all receipts for user {UserId}", id);
            
            var receipts = await _financeService.GetAllReceiptsAsync(id);
            
            _logger.LogInformation("Retrieved {Count} receipts for user {UserId}", 
                receipts.Count, id);
            return Ok(receipts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting receipts for user {UserId}", id);
            return StatusCode(500, new { message = "Fişler getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// ID'ye göre fiş getirir
    /// </summary>
    /// <param name="id">Fiş ID</param>
    /// <returns>Fiş detayı</returns>
    /// <response code="200">Fiş bulundu</response>
    /// <response code="404">Fiş bulunamadı</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ReceiptDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting receipt {ReceiptId}", id);
            
            var receipt = await _financeService.GetReceiptByIdAsync(id);
            if (receipt == null)
            {
                _logger.LogWarning("Receipt {ReceiptId} not found", id);
                return NotFound();
            }
            
            _logger.LogInformation("Retrieved receipt {ReceiptId}", id);
            return Ok(receipt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting receipt {ReceiptId}", id);
            return StatusCode(500, new { message = "Fiş getirilirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Yeni fiş oluşturur
    /// </summary>
    /// <param name="receiptDto">Fiş bilgileri</param>
    /// <returns>Oluşturulan fiş</returns>
    /// <response code="201">Fiş başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    [HttpPost]
    [ProducesResponseType(typeof(ReceiptDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] ReceiptDto receiptDto)
    {
        try
        {
            _logger.LogInformation("Creating new receipt for transaction {TransactionId}", 
                receiptDto.TransactionId);
            
            var created = await _financeService.CreateReceiptAsync(receiptDto);
            
            _logger.LogInformation("Created receipt {ReceiptId} for transaction {TransactionId}", 
                created.Id, created.TransactionId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create receipt: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating receipt for transaction {TransactionId}", 
                receiptDto.TransactionId);
            return StatusCode(500, new { message = "Fiş oluşturulurken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Fiş günceller
    /// </summary>
    /// <param name="id">Fiş ID</param>
    /// <param name="receiptDto">Güncellenecek fiş bilgileri</param>
    /// <returns>Güncellenen fiş</returns>
    /// <response code="200">Fiş başarıyla güncellendi</response>
    /// <response code="404">Fiş bulunamadı</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ReceiptDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] ReceiptDto receiptDto)
    {
        try
        {
            _logger.LogInformation("Updating receipt {ReceiptId}", id);
            
            var updated = await _financeService.UpdateReceiptAsync(id, receiptDto);
            
            _logger.LogInformation("Updated receipt {ReceiptId}", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Receipt {ReceiptId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating receipt {ReceiptId}", id);
            return StatusCode(500, new { message = "Fiş güncellenirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Fiş siler
    /// </summary>
    /// <param name="id">Fiş ID</param>
    /// <returns>Silme durumu</returns>
    /// <response code="204">Fiş başarıyla silindi</response>
    /// <response code="404">Fiş bulunamadı</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting receipt {ReceiptId}", id);
            
            await _financeService.DeleteReceiptAsync(id);
            
            _logger.LogInformation("Deleted receipt {ReceiptId}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Receipt {ReceiptId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting receipt {ReceiptId}", id);
            return StatusCode(500, new { message = "Fiş silinirken bir hata oluştu" });
        }
    }

    /// <summary>
    /// Fişe ait tüm kalemleri getirir
    /// </summary>
    /// <param name="receiptId">Fiş ID</param>
    /// <returns>Fiş kalemleri listesi</returns>
    /// <response code="200">Fiş kalemleri başarıyla getirildi</response>
    /// <response code="404">Fiş bulunamadı</response>
    [HttpGet("{receiptId}/items")]
    [ProducesResponseType(typeof(List<ReceiptItemDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetReceiptItems(Guid receiptId)
    {
        try
        {
            _logger.LogInformation("Getting items for receipt {ReceiptId}", receiptId);
            
            var items = await _financeService.GetReceiptItemsByReceiptIdAsync(receiptId);
            
            _logger.LogInformation("Retrieved {Count} items for receipt {ReceiptId}", 
                items.Count, receiptId);
            return Ok(items);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Receipt {ReceiptId} not found for getting items", receiptId);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting items for receipt {ReceiptId}", receiptId);
            return StatusCode(500, new { message = "Fiş kalemleri getirilirken bir hata oluştu" });
        }
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;


namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class ReceiptsController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public ReceiptsController(IFinanceService financeService)
    {
        _financeService = financeService;
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
        var receipts = await _financeService.GetAllReceiptsAsync(id);
        return Ok(receipts);
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
        var receipt = await _financeService.GetReceiptByIdAsync(id);
        return Ok(receipt);
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
        var created = await _financeService.CreateReceiptAsync(receiptDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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
        var updated = await _financeService.UpdateReceiptAsync(id, receiptDto);
        return Ok(updated);
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
        await _financeService.DeleteReceiptAsync(id);
        return NoContent();
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
        var items = await _financeService.GetReceiptItemsByReceiptIdAsync(receiptId);
        return Ok(items);
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TransactionsController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public TransactionsController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    /// <summary>
    /// Tüm işlemleri listeler
    /// </summary>
    /// <returns>İşlem listesi</returns>
    /// <response code="200">İşlemler başarıyla getirildi</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDto>), 200)]
    public async Task<IActionResult> GetAll(Guid id)
    {
        var transactions = await _financeService.GetTransactionsAsync(id);
        return Ok(transactions);
    }

    /// <summary>
    /// ID'ye göre işlem getirir
    /// </summary>
    /// <param name="id">İşlem ID</param>
    /// <returns>İşlem detayı</returns>
    /// <response code="200">İşlem bulundu</response>
    /// <response code="404">İşlem bulunamadı</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transaction = await _financeService.GetTransactionByIdAsync(id);
        return Ok(transaction);
    }

    /// <summary>
    /// Yeni işlem oluşturur
    /// </summary>
    /// <param name="transactionDto">İşlem bilgileri</param>
    /// <returns>Oluşturulan işlem</returns>
    /// <response code="201">İşlem başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] TransactionDto transactionDto)
    {
        var created = await _financeService.CreateTransactionAsync(transactionDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
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
    [ProducesResponseType(typeof(TransactionDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] TransactionDto transactionDto)
    {
        var updated = await _financeService.UpdateTransactionAsync(id, transactionDto);
        return Ok(updated);
    }

    /// <summary>
    /// İşlem siler
    /// </summary>
    /// <param name="id">İşlem ID</param>
    /// <returns>Silme durumu</returns>
    /// <response code="204">İşlem başarıyla silindi</response>
    /// <response code="404">İşlem bulunamadı</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _financeService.DeleteTransactionAsync(id);
        return NoContent();
    }
} 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Finance;

namespace Wallet.Api.Controllers;

[Authorize]
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IFinanceService _financeService;

    public CategoriesController(IFinanceService financeService)
    {
        _financeService = financeService;
    }

    /// <summary>
    /// Tüm kategorileri listeler
    /// </summary>
    /// <returns>Kategori listesi</returns>
    /// <response code="200">Kategoriler başarıyla getirildi</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _financeService.GetAllCategoriesAsync();
        return Ok(categories);
    }

    /// <summary>
    /// ID'ye göre kategori getirir
    /// </summary>
    /// <param name="id">Kategori ID</param>
    /// <returns>Kategori detayı</returns>
    /// <response code="200">Kategori bulundu</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var category = await _financeService.GetCategoryByIdAsync(id);
        return Ok(category);
    }

    /// <summary>
    /// Yeni kategori oluşturur
    /// </summary>
    /// <param name="categoryDto">Kategori bilgileri</param>
    /// <returns>Oluşturulan kategori</returns>
    /// <response code="201">Kategori başarıyla oluşturuldu</response>
    /// <response code="400">Geçersiz veri</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CategoryDto categoryDto)
    {
        var created = await _financeService.CreateCategoryAsync(categoryDto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Kategori günceller
    /// </summary>
    /// <param name="id">Kategori ID</param>
    /// <param name="categoryDto">Güncellenecek kategori bilgileri</param>
    /// <returns>Güncellenen kategori</returns>
    /// <response code="200">Kategori başarıyla güncellendi</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CategoryDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDto categoryDto)
    {
        var updated = await _financeService.UpdateCategoryAsync(id, categoryDto);
        return Ok(updated);
    }

    /// <summary>
    /// Kategori siler
    /// </summary>
    /// <param name="id">Kategori ID</param>
    /// <returns>Silme durumu</returns>
    /// <response code="204">Kategori başarıyla silindi</response>
    /// <response code="404">Kategori bulunamadı</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _financeService.DeleteCategoryAsync(id);
        return NoContent();
    }
} 
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
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("Categories")]
public class CategoriesController : ControllerBase
{
    private readonly IFinanceService _financeService;
    private readonly ILogger<CategoriesController> _logger;

    public CategoriesController(
        IFinanceService financeService,
        ILogger<CategoriesController> logger)
    {
        _financeService = financeService;
        _logger = logger;
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
        try
        {
            _logger.LogInformation("Getting all categories");
            
            var categories = await _financeService.GetAllCategoriesAsync();
            
            _logger.LogInformation("Retrieved {Count} categories", categories.Count);
            return Ok(categories);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting categories");
            return StatusCode(500, new { message = "Kategoriler getirilirken bir hata oluştu" });
        }
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
        try
        {
            _logger.LogInformation("Getting category {CategoryId}", id);
            
            var category = await _financeService.GetCategoryByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category {CategoryId} not found", id);
                return NotFound();
            }
            
            _logger.LogInformation("Retrieved category {CategoryId}", id);
            return Ok(category);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting category {CategoryId}", id);
            return StatusCode(500, new { message = "Kategori getirilirken bir hata oluştu" });
        }
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
        try
        {
            _logger.LogInformation("Creating new category {CategoryName}", categoryDto.Name);
            
            var created = await _financeService.CreateCategoryAsync(categoryDto);
            
            _logger.LogInformation("Created category {CategoryId} with name {CategoryName}", 
                created.Id, created.Name);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (BadRequestException ex)
        {
            _logger.LogWarning(ex, "Failed to create category: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category {CategoryName}", categoryDto.Name);
            return StatusCode(500, new { message = "Kategori oluşturulurken bir hata oluştu" });
        }
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
        try
        {
            _logger.LogInformation("Updating category {CategoryId}", id);
            
            var updated = await _financeService.UpdateCategoryAsync(id, categoryDto);
            
            _logger.LogInformation("Updated category {CategoryId}", id);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Category {CategoryId} not found for update", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return StatusCode(500, new { message = "Kategori güncellenirken bir hata oluştu" });
        }
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
        try
        {
            _logger.LogInformation("Deleting category {CategoryId}", id);
            
            await _financeService.DeleteCategoryAsync(id);
            
            _logger.LogInformation("Deleted category {CategoryId}", id);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex, "Category {CategoryId} not found for deletion", id);
            return NotFound(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            return StatusCode(500, new { message = "Kategori silinirken bir hata oluştu" });
        }
    }
} 
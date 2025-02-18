using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Messages;

namespace Wallet.Api.Controllers;

/// <summary>
/// Mesajlaşma sistemi için API endpoint'leri
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[ApiVersion("1.0")]
[Tags("Messages")] // Swagger'da ayrı bir bölüm olarak gösterilecek
public class MessagesController : ControllerBase
{
    private readonly IMessageService _messageService;

    public MessagesController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    /// <summary>
    /// Yeni bir mesaj gönderir
    /// </summary>
    /// <param name="messageDto">Mesaj detayları</param>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/messages
    ///     {
    ///         "receiverUsername": "johndoe",
    ///         "subject": "Meeting Tomorrow",
    ///         "content": "Hi John, can we meet tomorrow at 2 PM?",
    ///         "parentMessageId": null
    ///     }
    /// </remarks>
    /// <response code="200">Mesaj başarıyla gönderildi</response>
    /// <response code="400">Geçersiz istek veya alıcı bulunamadı</response>
    /// <response code="401">Yetkilendirme başarısız</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageDto>> SendMessage([FromBody] SendMessageDto messageDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var message = await _messageService.SendMessageAsync(userId, messageDto);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gelen kutusu mesajlarını getirir
    /// </summary>
    /// <remarks>
    /// Kullanıcının gelen kutusundaki tüm mesajları tarih sırasına göre getirir.
    /// Okunmamış mesajlar önce gösterilir.
    /// </remarks>
    /// <response code="200">Mesajlar başarıyla getirildi</response>
    /// <response code="401">Yetkilendirme başarısız</response>
    [HttpGet("inbox")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<MessageDto>>> GetInboxMessages()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var messages = await _messageService.GetInboxMessagesAsync(userId);
        return Ok(messages);
    }

    /// <summary>
    /// Gönderilen mesajları getirir
    /// </summary>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MessageDto>>> GetSentMessages()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var messages = await _messageService.GetSentMessagesAsync(userId);
        return Ok(messages);
    }

    /// <summary>
    /// Belirli bir mesajı getirir
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MessageDto>> GetMessage(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var message = await _messageService.GetMessageByIdAsync(id, userId);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesaj zincirini getirir
    /// </summary>
    [HttpGet("{id}/thread")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<MessageDto>>> GetMessageThread(Guid id)
    {
        try
        {
            var messages = await _messageService.GetMessageThreadAsync(id);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesajı okundu olarak işaretler
    /// </summary>
    [HttpPut("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _messageService.MarkAsReadAsync(id, userId);
            return Ok(new { message = "Message marked as read" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesajı okunmadı olarak işaretler
    /// </summary>
    [HttpPut("{id}/unread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsUnread(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _messageService.MarkAsUnreadAsync(id, userId);
            return Ok(new { message = "Message marked as unread" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesajı siler
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMessage(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _messageService.DeleteMessageAsync(id, userId);
            return Ok(new { message = "Message deleted" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesaj zincirini siler
    /// </summary>
    [HttpDelete("{id}/thread")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteThread(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            await _messageService.DeleteThreadAsync(id, userId);
            return Ok(new { message = "Thread deleted" });
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
} 
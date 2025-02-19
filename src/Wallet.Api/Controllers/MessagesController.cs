using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wallet.Services.DTOs.Messages;
using Microsoft.EntityFrameworkCore;
using Wallet.Services.DTOs.Auth;
using Wallet.Services.Abstract;
using Microsoft.Extensions.Configuration;

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
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;

    public MessagesController(
        IMessageService messageService,
        IAuthService authService,
        IConfiguration configuration)
    {
        _messageService = messageService;
        _authService = authService;
        _configuration = configuration;
    }

    /// <summary>
    /// Yeni mesaj gönderir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     POST /api/messages
    ///     Content-Type: multipart/form-data
    ///     {
    ///         "receiverUsername": "johndoe",
    ///         "subject": "Meeting Tomorrow",
    ///         "content": "Hi John, can we meet tomorrow at 2 PM?",
    ///         "parentMessageId": null,
    ///         "attachment": (binary file)
    ///     }
    /// </remarks>
    /// <response code="200">Mesaj başarıyla gönderildi</response>
    /// <response code="400">Geçersiz istek veya alıcı bulunamadı</response>
    /// <response code="401">Yetkilendirme başarısız</response>
    [HttpPost]
    [ProducesResponseType(typeof(MessageDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MessageDto>> SendMessage([FromForm] SendMessageDto messageDto)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            if (messageDto.Attachment != null)
            {
                var maxFileSizeInMB = _configuration.GetValue<int>("FileSettings:MaxFileSizeInMB");
                if (messageDto.Attachment.Length > maxFileSizeInMB * 1024 * 1024)
                    return BadRequest(new { message = $"File size exceeds {maxFileSizeInMB}MB limit" });

                var allowedTypes = _configuration.GetSection("FileSettings:AllowedFileTypes").Get<string[]>();
                if (!allowedTypes.Contains(messageDto.Attachment.ContentType))
                    return BadRequest(new { message = "Invalid file type" });
            }

            var message = await _messageService.SendMessageAsync(userId, messageDto);
            return Ok(message);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Gelen kutusu mesajlarını listeler
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/messages/inbox
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
    /// Gönderilen mesajları listeler
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/messages/sent
    /// </remarks>
    /// <response code="200">Mesajlar başarıyla getirildi</response>
    [HttpGet("sent")]
    [ProducesResponseType(typeof(List<MessageDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MessageDto>>> GetSentMessages()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
        var messages = await _messageService.GetSentMessagesAsync(userId);
        return Ok(messages);
    }

    /// <summary>
    /// Mesaj detayını getirir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/messages/{id}
    /// </remarks>
    /// <response code="200">Mesaj başarıyla getirildi</response>
    /// <response code="404">Mesaj bulunamadı</response>
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
    /// Mesaj ekini indirir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/messages/attachments/{id}
    /// </remarks>
    /// <response code="200">Dosya başarıyla indirildi</response>
    /// <response code="404">Dosya bulunamadı</response>
    [HttpGet("attachments/{id}")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadAttachment(Guid id)
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var (fileContents, fileName, contentType) = await _messageService.GetAttachmentByIdAsync(id, userId);
            return File(fileContents, contentType, fileName);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mesajı okundu olarak işaretler
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/messages/{id}/read
    /// </remarks>
    /// <response code="200">Mesaj başarıyla okundu</response>
    /// <response code="404">Mesaj bulunamadı</response>
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
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     PUT /api/messages/{id}/unread
    /// </remarks>
    /// <response code="200">Mesaj başarıyla okunmadı</response>
    /// <response code="404">Mesaj bulunamadı</response>
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
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     DELETE /api/messages/{id}
    /// </remarks>
    /// <response code="200">Mesaj başarıyla silindi</response>
    /// <response code="404">Mesaj bulunamadı</response>
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
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     DELETE /api/messages/{id}/thread
    /// </remarks>
    /// <response code="200">Mesaj zincirini başarıyla silindi</response>
    /// <response code="404">Mesaj zinciri bulunamadı</response>
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

    /// <summary>
    /// Kullanıcı listesini getirir
    /// </summary>
    /// <remarks>
    /// Örnek istek:
    /// 
    ///     GET /api/messages/users
    /// </remarks>
    /// <response code="200">Kullanıcılar başarıyla getirildi</response>
    /// <response code="401">Yetkilendirme başarısız</response>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserInfoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<UserInfoDto>>> GetUsers()
    {
        try
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var currentUser = await _authService.GetUserInfoAsync(userId);

            // Şu an için basit bir liste dönüyoruz, 
            // ileride pagination, filtreleme gibi özellikler eklenebilir
            var users = await _messageService.GetAllUsersAsync();

            return Ok(users.Where(u => u.Id != currentUser.Id).ToList());
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
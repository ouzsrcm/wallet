using Wallet.Services.DTOs.Messages;
using Wallet.Services.DTOs.Auth;
namespace Wallet.Services.Abstract;

public interface IMessageService
{
    // Mesaj gönderme
    Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto messageDto);
    
    // Mesaj okuma
    Task<MessageDto> GetMessageByIdAsync(Guid messageId, Guid userId);
    Task<List<MessageDto>> GetInboxMessagesAsync(Guid userId);
    Task<List<MessageDto>> GetSentMessagesAsync(Guid userId);
    Task<List<MessageDto>> GetMessageThreadAsync(Guid messageId);
    
    // Mesaj durumu
    Task MarkAsReadAsync(Guid messageId, Guid userId);
    Task MarkAsUnreadAsync(Guid messageId, Guid userId);
    
    // Mesaj silme
    Task DeleteMessageAsync(Guid messageId, Guid userId);
    Task DeleteThreadAsync(Guid messageId, Guid userId);

    Task<List<UserInfoDto>> GetAllUsersAsync();

    /// <summary>
    /// Mesaj ekini ID'ye göre getirir
    /// </summary>
    Task<(byte[] FileContents, string FileName, string ContentType)> GetAttachmentByIdAsync(Guid attachmentId, Guid userId);
} 
namespace Wallet.Services.DTOs.Messages;

/// <summary>
/// Yeni mesaj gönderme için kullanılan DTO
/// </summary>
public class SendMessageDto
{
    /// <summary>
    /// Alıcı kullanıcının kullanıcı adı
    /// </summary>
    /// <example>johndoe</example>
    public required string ReceiverUsername { get; set; }

    /// <summary>
    /// Mesajın konusu
    /// </summary>
    /// <example>Meeting Tomorrow</example>
    public required string Subject { get; set; }

    /// <summary>
    /// Mesajın içeriği
    /// </summary>
    /// <example>Hi John, can we meet tomorrow at 2 PM?</example>
    public required string Content { get; set; }

    /// <summary>
    /// Yanıt verilen mesajın kimliği (yanıt için)
    /// </summary>
    public Guid? ParentMessageId { get; set; }
} 
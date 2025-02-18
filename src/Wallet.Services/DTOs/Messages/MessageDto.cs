namespace Wallet.Services.DTOs.Messages;

/// <summary>
/// Mesaj detaylarını içeren DTO
/// </summary>
public class MessageDto
{
    /// <summary>
    /// Mesajın benzersiz kimliği
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gönderen kullanıcının kimliği
    /// </summary>
    public Guid SenderId { get; set; }

    /// <summary>
    /// Gönderen kullanıcının kullanıcı adı
    /// </summary>
    public string SenderUsername { get; set; } = null!;

    /// <summary>
    /// Gönderen kullanıcının tam adı
    /// </summary>
    public string SenderFullName { get; set; } = null!;

    public Guid ReceiverId { get; set; }
    public string ReceiverFullName { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Content { get; set; } = null!;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public Guid? ParentMessageId { get; set; }
    public List<MessageDto>? Replies { get; set; }
} 
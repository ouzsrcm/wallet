using Wallet.Entities.Base.Concrete;

namespace Wallet.Entities.EntityObjects;

public class Message : SoftDeleteEntity
{
    public Guid SenderId { get; set; }
    public User? Sender { get; set; }
    
    public Guid ReceiverId { get; set; }
    public User? Receiver { get; set; }
    
    public required string Subject { get; set; }
    public required string Content { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    
    // Yanıt zinciri için
    public Guid? ParentMessageId { get; set; }
    public Message? ParentMessage { get; set; }
    public ICollection<Message>? Replies { get; set; }
} 
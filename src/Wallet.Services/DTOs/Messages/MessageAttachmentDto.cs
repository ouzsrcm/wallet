namespace Wallet.Services.DTOs.Messages;

public class MessageAttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
} 
using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Messages;
using Wallet.Services.UnitOfWorkBase.Abstract;
using Wallet.Services.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
namespace Wallet.Services.Concrete;

public class MessageService : IMessageService
{
    private readonly IPersonUnitOfWork _unitOfWork;
    private readonly IConfiguration _configuration;
    private readonly IFileStorageService _fileStorageService;

    public MessageService(
        IPersonUnitOfWork unitOfWork, 
        IConfiguration configuration,
        IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _fileStorageService = fileStorageService;
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto messageDto)
    {
        // Alıcıyı bul
        var receiver = await _unitOfWork.Users
            .GetWhere(u => u.Credential!.Username == messageDto.ReceiverUsername)
            .FirstOrDefaultAsync() 
            ?? throw new Exception("Receiver not found");

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = receiver.Id,
            Subject = messageDto.Subject,
            Content = messageDto.Content,
            ParentMessageId = messageDto.ParentMessageId,
            CreatedBy = senderId.ToString()
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

        // Eğer dosya eki varsa kaydet
        if (messageDto.Attachment != null)
        {
            var (fileName, filePath) = await _fileStorageService.SaveFileAsync(
                messageDto.Attachment, 
                "messages"
            );

            var attachment = new MessageAttachment
            {
                MessageId = message.Id,
                FileName = messageDto.Attachment.FileName,
                ContentType = messageDto.Attachment.ContentType,
                FileSize = messageDto.Attachment.Length,
                FilePath = fileName,
                CreatedBy = senderId.ToString()
            };

            await _unitOfWork.MessageAttachments.AddAsync(attachment);
            await _unitOfWork.SaveChangesAsync();
        }

        return await GetMessageByIdAsync(message.Id, senderId);
    }

    public async Task<MessageDto> GetMessageByIdAsync(Guid messageId, Guid userId)
    {
        var message = await (from msg in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on msg.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on msg.ReceiverId equals receiver.Id
            join senderCred in _unitOfWork.UserCredentials.GetAll()
                on sender.Id equals senderCred.UserId
            where msg.Id == messageId && 
                  (msg.SenderId == userId || msg.ReceiverId == userId)
            select new MessageDto
            {
                Id = msg.Id,
                SenderId = msg.SenderId,
                SenderUsername = senderCred.Username,
                SenderFullName = sender.Person.FirstName + " " + sender.Person.LastName,
                ReceiverId = msg.ReceiverId,
                ReceiverFullName = receiver.Person.FirstName + " " + receiver.Person.LastName,
                Subject = msg.Subject,
                Content = msg.Content,
                IsRead = msg.IsRead,
                ReadAt = msg.ReadAt,
                CreatedDate = msg.CreatedDate,
                ParentMessageId = msg.ParentMessageId
            }).FirstOrDefaultAsync() 
            ?? throw new Exception("Message not found");

        if (message.ReceiverId == userId && !message.IsRead)
        {
            await MarkAsReadAsync(messageId, userId);
        }

        return message;
    }

    public async Task<List<MessageDto>> GetInboxMessagesAsync(Guid userId)
    {
        return await (from message in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on message.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on message.ReceiverId equals receiver.Id
            join senderCred in _unitOfWork.UserCredentials.GetAll()
                on sender.Id equals senderCred.UserId
            where message.ReceiverId == userId && !message.IsDeleted
            orderby message.CreatedDate descending
            select new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderUsername = senderCred.Username,
                SenderFullName = sender.Person.FirstName + " " + sender.Person.LastName,
                ReceiverId = message.ReceiverId,
                ReceiverFullName = receiver.Person.FirstName + " " + receiver.Person.LastName,
                Subject = message.Subject,
                Content = message.Content,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                CreatedDate = message.CreatedDate,
                ParentMessageId = message.ParentMessageId
            }).ToListAsync();
    }

    public async Task<List<MessageDto>> GetSentMessagesAsync(Guid userId)
    {
        return await (from message in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on message.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on message.ReceiverId equals receiver.Id
            join senderCred in _unitOfWork.UserCredentials.GetAll()
                on sender.Id equals senderCred.UserId
            where message.SenderId == userId && !message.IsDeleted
            orderby message.CreatedDate descending
            select new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderUsername = senderCred.Username,
                SenderFullName = sender.Person.FirstName + " " + sender.Person.LastName,
                ReceiverId = message.ReceiverId,
                ReceiverFullName = receiver.Person.FirstName + " " + receiver.Person.LastName,
                Subject = message.Subject,
                Content = message.Content,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                CreatedDate = message.CreatedDate,
                ParentMessageId = message.ParentMessageId
            }).ToListAsync();
    }

    public async Task<List<MessageDto>> GetMessageThreadAsync(Guid messageId)
    {
        return await (from message in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on message.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on message.ReceiverId equals receiver.Id
            where message.ParentMessageId == messageId || message.Id == messageId
            orderby message.CreatedDate
            select new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderFullName = sender.Person.FirstName + " " + sender.Person.LastName,
                ReceiverId = message.ReceiverId,
                ReceiverFullName = receiver.Person.FirstName + " " + receiver.Person.LastName,
                Subject = message.Subject,
                Content = message.Content,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                CreatedDate = message.CreatedDate,
                ParentMessageId = message.ParentMessageId
            }).ToListAsync();
    }

    public async Task MarkAsReadAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages
            .GetWhere(m => m.Id == messageId && m.ReceiverId == userId)
            .FirstOrDefaultAsync()
            ?? throw new Exception("Message not found");

        message.IsRead = true;
        message.ReadAt = DateTime.UtcNow;
        message.ModifiedBy = userId.ToString();
        message.ModifiedDate = DateTime.UtcNow;

        await _unitOfWork.Messages.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task MarkAsUnreadAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages
            .GetWhere(m => m.Id == messageId && m.ReceiverId == userId)
            .FirstOrDefaultAsync()
            ?? throw new Exception("Message not found");

        message.IsRead = false;
        message.ReadAt = null;
        message.ModifiedBy = userId.ToString();
        message.ModifiedDate = DateTime.UtcNow;

        await _unitOfWork.Messages.UpdateAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteMessageAsync(Guid messageId, Guid userId)
    {
        var message = await _unitOfWork.Messages
            .GetWhere(m => m.Id == messageId && 
                          (m.SenderId == userId || m.ReceiverId == userId))
            .FirstOrDefaultAsync()
            ?? throw new Exception("Message not found");

        await _unitOfWork.Messages.SoftDeleteAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteThreadAsync(Guid messageId, Guid userId)
    {
        var messages = await _unitOfWork.Messages
            .GetWhere(m => (m.ParentMessageId == messageId || m.Id == messageId) &&
                          (m.SenderId == userId || m.ReceiverId == userId))
            .ToListAsync();

        if (!messages.Any())
            throw new Exception("No messages found to delete");

        foreach (var message in messages)
        {
            message.IsDeleted = true;
            message.DeletedAt = DateTime.UtcNow;
            message.DeletedByUserId = userId.ToString();
            message.ModifiedDate = DateTime.UtcNow;
            message.ModifiedBy = userId.ToString();
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<UserInfoDto>> GetAllUsersAsync()
    {
        return await (from user in _unitOfWork.Users.GetAll()
            join person in _unitOfWork.Persons.GetAll()
                on user.PersonId equals person.Id
            join credential in _unitOfWork.UserCredentials.GetAll()
                on user.Id equals credential.UserId
            where !user.IsDeleted
            select new UserInfoDto
            {
                Id = user.Id,
                Username = credential.Username,
                FirstName = person.FirstName,
                LastName = person.LastName
            }).ToListAsync();
    }

    public async Task<MessageAttachmentDto> UploadAttachmentAsync(Guid messageId, IFormFile file)
    {
        var message = await _unitOfWork.Messages
            .GetByIdAsync(messageId) 
            ?? throw new Exception("Message not found");

        var attachmentPath = _configuration["FileStorage:AttachmentPath"]
            ?? throw new Exception("Attachment path not configured");

        // Güvenli dosya adı oluştur
        var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
        var filePath = Path.Combine(attachmentPath, fileName);

        // Dizin yoksa oluştur
        Directory.CreateDirectory(attachmentPath);

        // Dosyayı kaydet
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var attachment = new MessageAttachment
        {
            MessageId = messageId,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FilePath = fileName,
            CreatedBy = message.SenderId.ToString()
        };

        await _unitOfWork.MessageAttachments.AddAsync(attachment);
        await _unitOfWork.SaveChangesAsync();

        return new MessageAttachmentDto
        {
            Id = attachment.Id,
            FileName = attachment.FileName,
            ContentType = attachment.ContentType,
            FileSize = attachment.FileSize
        };
    }

    public async Task<List<MessageAttachmentDto>> GetMessageAttachmentsAsync(Guid messageId, Guid userId)
    {
        // Önce mesajı kontrol et
        var message = await _unitOfWork.Messages
            .GetByIdAsync(messageId) 
            ?? throw new Exception("Message not found");

        // Kullanıcının yetkisi var mı kontrol et
        if (message.SenderId != userId && message.ReceiverId != userId)
            throw new UnauthorizedAccessException("You don't have permission to access this message");

        // Ekleri getir
        return await (from a in _unitOfWork.MessageAttachments.GetAll()
            where a.MessageId == messageId && !a.IsDeleted
            select new MessageAttachmentDto
            {
                Id = a.Id,
                FileName = a.FileName,
                ContentType = a.ContentType,
                FileSize = a.FileSize
            }).ToListAsync();
    }

    public async Task<(byte[] FileContents, string FileName, string ContentType)> DownloadAttachmentAsync(Guid attachmentId)
    {
        // Eki bul
        var attachment = await _unitOfWork.MessageAttachments
            .GetByIdAsync(attachmentId) 
            ?? throw new Exception("Attachment not found");

        // Dosyayı getir
        var fileContents = await _fileStorageService.GetFileAsync(attachment.FilePath, "attachments");

        if (fileContents == null || fileContents.Length == 0)
            throw new Exception("File content is empty");

        return (fileContents, attachment.FileName, attachment.ContentType);
    }

    public async Task<(byte[] FileContents, string FileName, string ContentType)> GetAttachmentByIdAsync(Guid attachmentId, Guid userId)
    {
        // Eki ve ilgili mesaj bilgilerini tek sorguda getir
        var attachment = await (from a in _unitOfWork.MessageAttachments.GetAll()
            join m in _unitOfWork.Messages.GetAll() 
                on a.MessageId equals m.Id
            where a.Id == attachmentId 
                && !a.IsDeleted 
                && (m.SenderId == userId || m.ReceiverId == userId)
            select new 
            {
                a.Id,
                a.FileName,
                a.ContentType,
                a.FilePath,
                m.SenderId,
                m.ReceiverId
            }).FirstOrDefaultAsync() 
            ?? throw new Exception("Attachment not found");

        // Dosyayı getir
        var fileContents = await _fileStorageService.GetFileAsync(attachment.FilePath, "attachments");

        if (fileContents == null || fileContents.Length == 0)
            throw new Exception("File content is empty");

        return (fileContents, attachment.FileName, attachment.ContentType);
    }
} 
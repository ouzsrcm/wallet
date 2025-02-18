using Microsoft.EntityFrameworkCore;
using Wallet.Entities.EntityObjects;
using Wallet.Services.Abstract;
using Wallet.Services.DTOs.Messages;
using Wallet.Services.UnitOfWorkBase.Abstract;

namespace Wallet.Services.Concrete;

public class MessageService : IMessageService
{
    private readonly IPersonUnitOfWork _unitOfWork;

    public MessageService(IPersonUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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

        return await GetMessageByIdAsync(message.Id, senderId);
    }

    public async Task<MessageDto> GetMessageByIdAsync(Guid messageId, Guid userId)
    {
        var message = await (from msg in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on msg.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on msg.ReceiverId equals receiver.Id
            where msg.Id == messageId && 
                  (msg.SenderId == userId || msg.ReceiverId == userId)
            select new MessageDto
            {
                Id = msg.Id,
                SenderId = msg.SenderId,
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
            where message.ReceiverId == userId && !message.IsDeleted
            orderby message.CreatedDate descending
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

    public async Task<List<MessageDto>> GetSentMessagesAsync(Guid userId)
    {
        return await (from message in _unitOfWork.Messages.GetAll()
            join sender in _unitOfWork.Users.GetAll() 
                on message.SenderId equals sender.Id
            join receiver in _unitOfWork.Users.GetAll() 
                on message.ReceiverId equals receiver.Id
            where message.SenderId == userId && !message.IsDeleted
            orderby message.CreatedDate descending
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
} 
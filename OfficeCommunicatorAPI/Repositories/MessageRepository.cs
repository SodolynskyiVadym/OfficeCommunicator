using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;
using static OfficeCommunicatorAPI.Services.CommunicatorHub;

namespace OfficeCommunicatorAPI.Repositories;

public class MessageRepository
{
    private OfficeDbContext _dbContext;
    public IMapper _mapper;
    
    public MessageRepository(OfficeDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }
    
    
    public async Task<Message?> GetByIdAsync(int id)
    {
        return await _dbContext.Messages.FindAsync(id);
    }

    public async Task<bool> AddMessageGroupAsync(Message message)
    {
        Group? group = await _dbContext.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.ChatId == message.ChatId);
        if (group == null || !group.Users.Any(u => u.Id == message.UserId)) return false;

        await _dbContext.AddAsync(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> AddMessageContactAsync(Message message)
    {
        Contact? contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.ChatId == message.ChatId);
        if(contact == null || (contact.UserId != message.UserId && contact.AssociatedUserId != message.UserId)) return false;

        await _dbContext.AddAsync(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    // public async Task<string> AddAsyncWithCheck(Message message, int groupContactId)
    // {
    //     string groupContactName = string.Empty;
    //
    //     Group? group = await _dbContext.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == groupContactId);
    //     if (group != null && !group.Users.Any(u => u.Id == message.UserId)) return string.Empty;
    //     if (group != null) GeneratorHubGroupName.GenerateGroupName(group);
    //
    //     if (string.IsNullOrEmpty(groupContactName))
    //     {
    //         Contact? contact = await _dbContext.Contacts.FindAsync(groupContactId);
    //         if (contact == null || (contact.UserId != message.UserId && contact.AssociatedUserId != message.UserId)) return string.Empty;
    //         groupContactName = GeneratorHubGroupName.GenerateContactName(contact);
    //     }
    //
    //     await _dbContext.AddAsync(message);
    //     bool result = await _dbContext.SaveChangesAsync() > 0;
    //     return result ? groupContactName : string.Empty;
    // }

    
    public async Task<bool> RemoveAsync(int messageId, int userId)
    {
        Message? message = await _dbContext.Messages.FindAsync(messageId);
        if (message == null) return false;
        
        if (message.UserId != userId) return false;
        
        _dbContext.Messages.Remove(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
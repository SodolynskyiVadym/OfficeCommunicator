using AutoMapper;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

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
    public async Task<bool> AddAsync(Message message)
    {
        await _dbContext.AddAsync(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    
    public async Task<bool> RemoveAsync(int messageId, int userId)
    {
        Message? message = await _dbContext.Messages.FindAsync(messageId);
        if (message == null) return false;
        
        if (message.UserId != userId) return false;
        
        _dbContext.Messages.Remove(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
using AutoMapper;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;
using OfficeCommunicatorAPI.Services.Checkers;

namespace OfficeCommunicatorAPI.Repositories;

public class MessageRepository
{
    private OfficeDbContext _dbContext;
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    public IMapper _mapper;
    
    public MessageRepository(OfficeDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _groupRepository = new GroupRepository(dbContext, mapper);
        _contactRepository = new ContactRepository(dbContext, mapper);
    }
    
    
    public async Task<Message?> GetByIdAsync(int id)
    {
        return await _dbContext.Messages.FindAsync(id);
    }

    public Task<Message?> GetByIdWithIncludeAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Message?> AddAsync(MessageDto messageDto, IChecker checker)
    {
        if (!await checker.CheckPermissionUser(messageDto.UserId, messageDto.ChatId)) return null;

        Message message = _mapper.Map<Message>(messageDto);
        await _dbContext.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        return message;
    }

    public Task<bool> UpdateAsync(MessageUpdateDto entity)
    {
        throw new NotImplementedException();
    }


    public async Task<bool> RemoveAsync(int messageId, int userId)
    {
        Message? message = await _dbContext.Messages.FindAsync(messageId);
        if (message == null) return false;
        
        if (message.UserId != userId) return false;
        
        _dbContext.Messages.Remove(message);
        return await _dbContext.SaveChangesAsync() > 0;
    }


    public Task<bool> DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}
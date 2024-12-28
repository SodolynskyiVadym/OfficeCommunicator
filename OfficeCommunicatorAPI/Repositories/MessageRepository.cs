using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Message?> GetByIdWithIncludeAsync(int id)
    {
        return await _dbContext.Messages.Include(m => m.Documents).FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Message?> AddAsync(MessageDto messageDto, IChecker checker)
    {
        if (!await checker.CheckPermissionUser(messageDto.UserId, messageDto.ChatId)) return null;

        Message message = _mapper.Map<Message>(messageDto);
        await _dbContext.AddAsync(message);
        await _dbContext.SaveChangesAsync();
        return message;
    }

    public async Task<Message?> UpdateAsync(MessageUpdateDto entity)
    {
        Message? message = await _dbContext.Messages.Include(m => m.Documents).FirstOrDefaultAsync(m => m.Id == entity.Id);
        if (message == null) return message;
        message.Content = entity.Content;
        return await _dbContext.SaveChangesAsync() > 0 ? message : null;
    }


    public async Task<Message?> RemoveAsync(int messageId, int userId)
    {
        Message? message = await _dbContext.Messages.Include(m => m.Documents).FirstOrDefaultAsync(m => m.Id == messageId);
        if (message == null) return message;
        
        if (message.UserId != userId) return null;
        
        _dbContext.Messages.Remove(message);
        ;

        return await _dbContext.SaveChangesAsync() > 0 ? message : null;
    }


    public async Task<Document?> GetDocumentByIdAsync(int documentId)
    {
        return await _dbContext.Documents.FindAsync(documentId);
    }

    public async Task<bool> RemoveDocumentAsync(int documentId)
    {
        Document? document = await _dbContext.Documents.FindAsync(documentId);
        if (document == null) return false;
        _dbContext.Documents.Remove(document);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
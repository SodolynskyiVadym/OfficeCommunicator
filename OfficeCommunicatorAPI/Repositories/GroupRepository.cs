﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories;

public class GroupRepository : IRepository<Group, GroupDto, GroupUpdateDto>
{
    private readonly OfficeDbContext _dbContext;
    private IMapper _mapper;

    public GroupRepository(IMapper mapper, OfficeDbContext dbContext)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }

    public async Task<Group?> GetByIdWithIncludeAsync(int id)
    {
        return await _dbContext.Groups
            .Include(c => c.Users)
            .Include(c => c.Admins)
            .Include(c => c.Chat)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Group?> GetByIdAsync(int id)
    {
        return await _dbContext.Groups.FindAsync(id);
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _dbContext.Groups.ToListAsync();
    }

    public async Task<Group> AddAsync(GroupDto groupDto)
    {
        Group group = _mapper.Map<Group>(groupDto);
        Chat chat = new Chat();
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();
        
        group.ChatId = chat.Id;
        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();
        return group;
    }

    public async Task<bool> UpdateAsync(GroupUpdateDto groupDto)
    {
        Group? group = await _dbContext.Groups.FindAsync(groupDto.Id);
        if (group == null) return false;
        
        group.Name = groupDto.Name;
        int result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Group? group = await _dbContext.Groups.FindAsync(id);
        if (group == null) return false;
        
        _dbContext.Groups.Remove(group);    
        int result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
}
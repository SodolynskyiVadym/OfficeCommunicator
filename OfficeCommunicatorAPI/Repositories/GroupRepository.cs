using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories;

public class GroupRepository : IRepository<Group, GroupDto, GroupUpdateDto>
{
    private readonly OfficeDbContext _dbContext;
    private readonly IMapper _mapper;

    public GroupRepository(OfficeDbContext dbContext, IMapper mapper)
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
            .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Group?> GetByIdAsync(int id)
    {
        return await _dbContext.Groups.FindAsync(id);
    }

    public async Task<Group?> GetGroupWithUsers(int groupId, int userId)
    {
        return await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Chat)
            .ThenInclude(c => c.Messages)
            .FirstOrDefaultAsync(g => g.Id == groupId && g.Users.FirstOrDefault(u => u.Id == userId) != null);
    }

    public async Task<List<Group>> GetAllAsync()
    {
        return await _dbContext.Groups.ToListAsync();
    }

    public async Task<Group> AddAsync(GroupDto groupDto)
    {
        Group group = _mapper.Map<Group>(groupDto);
        User? user = await _dbContext.Users.FindAsync(groupDto.UserAuthorId);
        if (user == null) throw new Exception("User not found");

        Chat chat = new Chat();
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        group.ChatId = chat.Id;
        group.Admins ??= new List<User>(){user};
        group.Users ??= new List<User>(){user};

        group.Admins.Add(user);
        group.Users.Add(user);
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
    
    
    public async Task<Group?> GetGroupWithUsers(int groupId)
    {
        return await _dbContext.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.Id == groupId);
    }
    
    public async Task<bool> IsUserAdmin(int groupId, int userId)
    {
        Group? group = await _dbContext.Groups.Include(g => g.Admins).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null) return false;
        
        return group.Admins.Any(a => a.Id == userId);
    }
}
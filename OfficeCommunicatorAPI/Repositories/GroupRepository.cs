using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories;

public class GroupRepository
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

    public async Task<Group?> GetGroupWithUsersAdmins(int groupId, int userId)
    {
        return await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Admins)
            .Include(g => g.Chat)
            .ThenInclude(c => c.Messages)
            .ThenInclude(m => m.Documents)
            .Include(g => g.Chat)
            .ThenInclude(c => c.Messages)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(g => g.Id == groupId && g.Users.FirstOrDefault(u => u.Id == userId) != null);
    }
    
   

    public async Task<List<Group>> GetAllAsync()
    {
        return await _dbContext.Groups.ToListAsync();
    }

    public async Task<Group?> AddAsync(GroupDto groupDto)
    {
        Group group = _mapper.Map<Group>(groupDto);
        User? user = await _dbContext.Users.FindAsync(groupDto.UserAuthorId);
        if (user == null) return null;

        Chat chat = new Chat() { Messages = new List<Message>()};

        group.Chat = chat;
        group.Admins ??= new List<User>(){user};
        group.Users ??= new List<User>(){user};

        await _dbContext.Groups.AddAsync(group);
        await _dbContext.SaveChangesAsync();
        return group;
    }
    
    
    public async Task<User?> AddUserToGroupAsync(int groupId, int userId, int adminUserId)
    {
        Console.WriteLine("Adding repository work");
        Group? group = await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Admins)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return null;
        Console.WriteLine("Group is not null");
        if (!group.Admins.Any(a => a.Id == adminUserId)) return null;
        Console.WriteLine("Admin is in group");
        if (group.Users.Any(u => u.Id == userId)) return null;
        Console.WriteLine("User is not in group");

        User? user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;
        Console.WriteLine("User is not null");

        group.Users.Add(user);

        if (await _dbContext.SaveChangesAsync() > 0) return user;
        else return null;
    }


    public async Task<bool> AddAdminAsync(int groupId, int userId, int adminUserId)
    {
        Group? group = await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Admins)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return false;
        if (!group.Admins.Any(a => a.Id == adminUserId)) return false;
        if (group.Admins.Any(a => a.Id == userId)) return false;
        if (!group.Users.Any(u => u.Id == userId)) return false;

        User? user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return false;

        group.Admins.Add(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }



    public async Task<List<Document>?> RemoveUserFromGroupAsync(int groupId, int userId)
    {
        Group? group = await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Admins)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return null;
        if (!group.Users.Any(u => u.Id == userId)) return null;
        User? user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return null;

        group.Users.Remove(user);
        group.Admins.Remove(user);

        if (group.Users.Count() == 0)
        {
            Chat? chat = _dbContext.Chats
                .Include(c => c.Messages)
                .ThenInclude(m => m.Documents)
                .FirstOrDefault(c => c.Id == group.ChatId);
            _dbContext.Groups.Remove(group);
            if (chat != null) _dbContext.Chats.Remove(chat);

            await _dbContext.SaveChangesAsync();
            return chat?.Messages.SelectMany(m => m.Documents).ToList();
        }

        if(!(await _dbContext.SaveChangesAsync() > 0)) return null;
        return new List<Document>();
    }

    public async Task<bool> RemoveUserFromGroupAsAdminAsync(int groupId, int userId, int adminUserId)
    {
        Group? group = await _dbContext.Groups
            .Include(g => g.Users)
            .Include(g => g.Admins)
            .FirstOrDefaultAsync(g => g.Id == groupId);

        if (group == null) return false;
        if (!group.Users.Any(u => u.Id == userId)) return false;
        if (!group.Admins.Any(a => a.Id == adminUserId)) return false;

        User? user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return false;

        group.Users.Remove(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }


    public async Task<bool> UpdateAsync(GroupUpdateDto groupDto)
    {
        Group? group = await _dbContext.Groups.FindAsync(groupDto.Id);
        if (group == null) return false;
        
        group.Name = groupDto.Name;
        int result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }


    public async Task<List<Document>?> DeleteAsync(int id)
    {
        Group? group = await _dbContext.Groups
            .Include(g => g.Chat)
            .ThenInclude(c => c.Messages)
            .ThenInclude(m => m.Documents)
            .FirstOrDefaultAsync(g => g.Id == id);
        if (group == null) return null;
        
        _dbContext.Groups.Remove(group);  
        _dbContext.Chats.Remove(group.Chat);
        if(!(await _dbContext.SaveChangesAsync() > 0)) return null;
        return group.Chat.Messages.SelectMany(m => m.Documents).ToList();
    }
    
    

    public async Task<bool> IsUserAdmin(int groupId, int userId)
    {
        Group? group = await _dbContext.Groups.Include(g => g.Admins).FirstOrDefaultAsync(g => g.Id == groupId);
        if (group == null) return false;
        
        return group.Admins.Any(a => a.Id == userId);
    }


    public async Task<bool> IsUserGroup(int chatId, int userId)
    {
        Group? group = await _dbContext.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.ChatId == chatId);
        if (group == null) return false;
        return group.Users.Any(u => u.Id == userId);
    }
}
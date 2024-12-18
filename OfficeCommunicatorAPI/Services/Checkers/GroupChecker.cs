
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.Services.Checkers
{
    public class GroupChecker : IChecker
    {
        private readonly OfficeDbContext _dbContext;

        public GroupChecker(OfficeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckPermissionUser(int userId, int chatId)
        {
            Group? group = await _dbContext.Groups.Include(g => g.Users).FirstOrDefaultAsync(g => g.ChatId == chatId);
            if (group == null) return false;
            return group.Users.Any(u => u.Id == userId);
        }
    }
}

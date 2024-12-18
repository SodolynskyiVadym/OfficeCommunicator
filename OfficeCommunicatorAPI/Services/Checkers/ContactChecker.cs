
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.Services.Checkers
{
    public class ContactChecker : IChecker
    {
        private readonly OfficeDbContext _dbContext;

        public ContactChecker(OfficeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> CheckPermissionUser(int userId, int chatId)
        {
            Contact? contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.ChatId == chatId);
            if (contact == null || (contact.UserId != userId && contact.AssociatedUserId != userId)) return false;
            return true;
        }
    }
}

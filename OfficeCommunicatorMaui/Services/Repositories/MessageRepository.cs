using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services.Repositories
{
    public class MessageRepository
    {
        private readonly SqliteDbContext _dbContext;

        public MessageRepository(SqliteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<MessegeStorageModel>> GetUnsentMessagesAsync()
        {
            return await _dbContext.Messages.ToListAsync();
        }

        public async Task<int> AddMessage(MessegeStorageModel message)
        {
            await _dbContext.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message.Id;
        }

        public async Task<bool> RemoveMessage(int id)
        {
            MessegeStorageModel? message = await _dbContext.Messages.FindAsync(id);
            if (message == null) return false;

            _dbContext.Messages.Remove(message);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorMaui.DTO;

namespace OfficeCommunicatorMaui.Services.Repositories
{
    public class MessageRepository
    {
        private readonly SqliteDbContext _dbContext;

        public MessageRepository(SqliteDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<List<MessageStorageDto>> GetUnsentMessagesAsync()
        {
            return await _dbContext.Messages.ToListAsync();
        }

        public async Task<MessageStorageDto> AddMessage(MessageStorageDto message)
        {
            await _dbContext.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<bool> RemoveMessage(int id)
        {
            MessageStorageDto? message = await _dbContext.Messages.FindAsync(id);
            if (message == null) return false;

            _dbContext.Messages.Remove(message);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

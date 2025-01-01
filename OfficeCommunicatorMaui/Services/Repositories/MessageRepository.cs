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


        public async Task<List<MessageStorageModel>> GetUnsentMessagesAsync()
        {
            return await _dbContext.Messages.ToListAsync();
        }

        public async Task<MessageStorageModel> AddMessage(MessageStorageModel message)
        {
            await _dbContext.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            return message;
        }

        public async Task<bool> UpdateMessage(MessageStorageModel messageDto)
        {
            if (messageDto == null) return false;
            MessageStorageModel? message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.UniqueIdentifier.Equals(messageDto.UniqueIdentifier));
            if (message == null) return false;

            if (!string.IsNullOrEmpty(message.Content)) message.Content = messageDto.Content;
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveMessage(string uniqueIdentifier)
        {
            MessageStorageModel? message = await _dbContext.Messages.FirstOrDefaultAsync(m => m.UniqueIdentifier.Equals(uniqueIdentifier));
            if (message == null) return false;

            _dbContext.Messages.Remove(message);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveMessage(int id)
        {
            MessageStorageModel? message = await _dbContext.Messages.FindAsync(id);
            if (message == null) return false;

            _dbContext.Messages.Remove(message);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}

using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories
{
    public class DocumentRepository
    {
        private readonly OfficeDbContext _dbContext;

        public DocumentRepository(OfficeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Document?> GetByIdAsync(int id)
        {
            return await _dbContext.Documents.FindAsync(id);
        }


        public async Task<Document?> AddAsync(Document document)
        {
            await _dbContext.AddAsync(document);
            await _dbContext.SaveChangesAsync();
            return document;
        }

        public async Task<List<Document>> AddRangeAsync(List<Document> documents)
        {
            await _dbContext.AddRangeAsync(documents);
            await _dbContext.SaveChangesAsync();
            return documents;
        }

        public async Task<bool> RemoveAsync(int documentId)
        {
            Document? document = await _dbContext.Documents.FindAsync(documentId);
            if (document == null) return false;
            _dbContext.Remove(document);
            await _dbContext.SaveChangesAsync();
            return true;
        }
    }
}

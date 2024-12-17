using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorMaui.DTO;

namespace OfficeCommunicatorMaui.Services
{
    public class SqliteDbContext : DbContext
    {
        private readonly string _dbPath;
        
        public DbSet<MessageStorageDto> Messages { get; set; }


        public SqliteDbContext(string dbPath)
        {
            _dbPath = dbPath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Filename={_dbPath}");
        }
    }
}

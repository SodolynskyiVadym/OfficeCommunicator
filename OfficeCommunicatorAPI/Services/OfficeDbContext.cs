using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.Services;

public class OfficeDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Document> Documents { get; set; }

    public OfficeDbContext(DbContextOptions<OfficeDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UniqueName)
            .IsUnique();

        
        modelBuilder.Entity<Contact>()
            .HasIndex(c => new { c.UserId, c.AssociatedUserId })
            .IsUnique();

        modelBuilder.Entity<Group>()
            .HasIndex(g => g.UniqueIdentifier)
            .IsUnique();

        modelBuilder.Entity<Document>()
            .HasIndex(d => d.UniqueIdentifier);


        modelBuilder.Entity<User>()
                .HasMany(u => u.Groups)
                .WithMany(g => g.Users);


        modelBuilder.Entity<User>()
            .HasMany(u => u.Contacts)
            .WithOne()
            .HasForeignKey(c => c.UserId)
            .HasPrincipalKey(u => u.Id);
        
        modelBuilder.Entity<Contact>()
            .HasOne<User>(c => c.AssociatedUser)
            .WithMany()
            .HasForeignKey(c => c.AssociatedUserId)
            .HasPrincipalKey(u => u.Id)
            .OnDelete(DeleteBehavior.Restrict);
        

        modelBuilder.Entity<Group>()
            .HasMany(c => c.Admins)
            .WithMany();
        
        modelBuilder.Entity<Chat>()
            .HasMany<Message>(c => c.Messages)
            .WithOne()
            .HasForeignKey(m => m.ChatId)
            .HasPrincipalKey(c => c.Id);
        
        modelBuilder.Entity<Chat>()
            .HasMany<Contact>()
            .WithOne(c => c.Chat)
            .HasForeignKey(c => c.ChatId)
            .HasPrincipalKey(c => c.Id);
        
        modelBuilder.Entity<Group>()
            .HasOne<Chat>(g => g.Chat)
            .WithOne()
            .HasForeignKey<Group>(g => g.ChatId);
        
        modelBuilder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    
            
        modelBuilder.Entity<Message>()
            .HasMany(m => m.Documents)
            .WithOne()
            .HasForeignKey(d => d.MessageId)
            .HasPrincipalKey(m => m.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
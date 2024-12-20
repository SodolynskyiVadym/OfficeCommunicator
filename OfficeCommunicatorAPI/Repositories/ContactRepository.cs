﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Services;

namespace OfficeCommunicatorAPI.Repositories;

public class ContactRepository : IRepository<Contact, ContactDto, ContactUpdateDto>
{
    private readonly OfficeDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public ContactRepository(OfficeDbContext dbContext, IMapper mapper)
    {
        _mapper = mapper;
        _dbContext = dbContext;
    }
    
    public async Task<Contact?> GetByIdWithIncludeAsync(int id)
    {
        return await _dbContext.Contacts
            .Include(c => c.Chat)
            .ThenInclude(c => c.Messages)
            .Include(c => c.AssociatedUser)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Contact?> GetByIdAsync(int id)
    {
        return await _dbContext.Contacts.FindAsync(id);
    }

    public async Task<Contact?> GetByUserIdAndAssociatedUserIdWithIncludesAsync(int userId, int associatedUserId)
    {
        return await _dbContext.Contacts
            .Include(c => c.Chat)
            .ThenInclude(c => c.Messages)
            .ThenInclude(m => m.Documents)
            .Include(c => c.AssociatedUser)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AssociatedUserId == associatedUserId);
    }

    public async Task<Contact?> GetByUserIdAndAssociatedUserIdAsync(int userId, int associatedUserId)
    {
        return await _dbContext.Contacts
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AssociatedUserId == associatedUserId);
    }

    public async Task<List<Contact>> GetAllAsync()
    {
        return await _dbContext.Contacts.Include(c => c.AssociatedUser).ToListAsync();
    }

    public async Task<Contact> AddAsync(ContactDto contactDto)
    {
        if(contactDto.AssociatedUserId == contactDto.UserId) throw new Exception("Cannot add yourself as a contact");

        User? associatedUser = await _dbContext.Users.FindAsync(contactDto.AssociatedUserId);
        if (associatedUser == null) throw new Exception("Associated user not found");

        User? user = await _dbContext.Users.FindAsync(contactDto.UserId);
        if (user == null) throw new Exception("User not found");

        Chat chat = new Chat();
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();
        
        Contact contact = _mapper.Map<Contact>(contactDto);
        contact.ChatId = chat.Id;
        
        await _dbContext.Contacts.AddAsync(contact);
        
        Contact associatedContact = new Contact
        {
            UserId = contact.AssociatedUserId,
            AssociatedUserId = contact.UserId,
            ChatId = contact.ChatId
        };

        await _dbContext.Contacts.AddAsync(associatedContact);
        await _dbContext.SaveChangesAsync();

        return contact;
    }


    public async Task<(Contact?, Contact?)> AddContactAsync(int userId, int associatedUserId)
    {
        if (associatedUserId == userId) return (null, null);

        User? associatedUser = await _dbContext.Users.FindAsync(associatedUserId);
        if (associatedUser == null) return (null, null);

        User? user = await _dbContext.Users.FindAsync(userId);
        if (user == null) return (null, null);

        Chat chat = new Chat();
        await _dbContext.Chats.AddAsync(chat);
        await _dbContext.SaveChangesAsync();

        Contact contact = new Contact()
        {
            UserId = userId,
            AssociatedUserId = associatedUserId,
            ChatId = chat.Id
        };

        await _dbContext.Contacts.AddAsync(contact);

        Contact associatedContact = new Contact
        {
            UserId = contact.AssociatedUserId,
            AssociatedUserId = contact.UserId,
            ChatId = contact.ChatId
        };

        await _dbContext.Contacts.AddAsync(associatedContact);
        await _dbContext.SaveChangesAsync();

        return (contact, associatedContact);
    }

    public Task<bool> UpdateAsync(ContactUpdateDto entity)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Contact? contact = await _dbContext.Contacts.FindAsync(id);
        if (contact == null) return false;
        
        _dbContext.Contacts.Remove(contact);
        int result = await _dbContext.SaveChangesAsync();
        return result > 0;
    }
    
    
    public async Task<bool> IsUserContact(int chatId, int userId)
    {
        Contact? contact = await _dbContext.Contacts.FirstOrDefaultAsync(c => c.ChatId == chatId);
        if(contact == null || (contact.UserId != userId && contact.AssociatedUserId != userId)) return false;
        return true;
    }
}
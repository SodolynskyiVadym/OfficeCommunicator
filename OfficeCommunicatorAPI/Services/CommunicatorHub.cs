using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Repositories;

namespace OfficeCommunicatorAPI.Services;

[Authorize]
public class CommunicatorHub : Hub
{
    private readonly UserRepository _userRepository;
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    private readonly MessageRepository _messageRepository;
    private static int _counter;

    public CommunicatorHub(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper)
    {
        _messageRepository = new MessageRepository(dbContext, mapper);
        _userRepository = new UserRepository(dbContext, mapper, authHelper);
        _groupRepository = new GroupRepository(dbContext, mapper);
        _contactRepository = new ContactRepository(dbContext, mapper);
    }



    public override async Task OnConnectedAsync()
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        User? user = await _userRepository.GetByIdWithIncludeAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        foreach (var group in user.Groups) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(group.ChatId));
        foreach (var contact in user.Contacts) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateContactName(contact.ChatId));
        _counter++;
        Console.WriteLine($"A client connected {DateTime.Now}. Total clients: {_counter}");
        Console.WriteLine($"User was added to ${user.Groups.Count()} groups");
        await base.OnConnectedAsync();
    }



    public async Task JoinGroup(string communication, int communicationId)
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        if(communication == "group")
        {
            Group? group = await _groupRepository.GetGroupWithUsers(communicationId);
            if (group == null) throw new ArgumentException("Group not found");
            
            bool isUserMember = group.Users.Any(u => u.Id == userId);
            if (!isUserMember) throw new ArgumentException("User is not a member of the group");
            
            await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(group.ChatId));
        }else if (communication == "contact")
        {
            Contact? contact = await _contactRepository.GetByIdAsync(communicationId);
            if (contact == null) throw new ArgumentException("Contact not found");

            bool isUserContact = contact.UserId == userId || contact.AssociatedUserId == userId;
            if (!isUserContact) throw new ArgumentException("User is not a contact");

            await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateContactName(contact.ChatId));
        }else throw new ArgumentException("Invalid communication type");
    }
    

    public async Task SendMessageGroup(int chatId, string content)
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        Message message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            Content = content,
            Date = DateTime.Now
        };
        bool result = await _messageRepository.AddMessageGroupAsync(message);
        if (!result) throw new Exception("Message not sent because user is not a member of the group");
        
        await Clients.Group(GeneratorHubGroupName.GenerateGroupName(message.ChatId)).SendAsync("ReceiveGroupMessage", chatId, message);
    }

    public async Task SendMessageContact(int chatId, string content)
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        Message message = new Message
        {
            ChatId = chatId,
            UserId = userId,
            Content = content,
            Date = DateTime.Now
        };
        bool result = await _messageRepository.AddMessageContactAsync(message);
        if (!result) throw new Exception("Message not sent because user is not a contact");
        
        await Clients.Group(GeneratorHubGroupName.GenerateContactName(message.ChatId)).SendAsync("ReceiveMessage", chatId, message);
    }


    public async Task RemoveMessage(int communicationId, string communication, int messageId)
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");
        
        Message? message = await _messageRepository.GetByIdAsync(messageId);
        if (message == null || message.UserId != userId) throw new ArgumentException("Message not found or you aren't the author");

        string hubGroupName;
        if (communication == "group")
        {
            Group? group = await _groupRepository.GetByIdAsync(communicationId);
            if (group == null) throw new ArgumentException("Group not found");
            hubGroupName = GeneratorHubGroupName.GenerateGroupName(group.ChatId);
        }else if (communication == "contact")
        {
            Contact? contact = await _contactRepository.GetByIdAsync(communicationId);
            if (contact == null) throw new ArgumentException("Contact not found");
            hubGroupName = GeneratorHubGroupName.GenerateContactName(contact.ChatId);
        }else throw new ArgumentException("Invalid communication type");


        bool result = await _messageRepository.RemoveAsync(messageId, userId);
        if (!result) throw new Exception("Message not removed");
        
        await Clients.Group(hubGroupName).SendAsync("RemoveMessage", communicationId, communication, messageId);
    }

    //public async Task AddItem(string shoppingListId, string itemName)
    //{
    //    if (_groupIdShoppingLists.TryGetValue(shoppingListId, out var list))
    //    {
    //        list.Add(itemName);
    //        await Clients.Group(shoppingListId).SendAsync("ReceiveShoppingList", list);
    //    }
    //}

    //public async Task RemoveItem(string shoppingListId, string itemName)
    //{
    //    if (_groupIdShoppingLists.TryGetValue(shoppingListId, out var list))
    //    {
    //        Console.WriteLine(list.Contains(itemName));
    //        list.Remove(itemName);
    //        Console.WriteLine(list.Contains(itemName));
    //        await Clients.Group(shoppingListId).SendAsync("ReceiveShoppingList", list);
    //    }
    //}

    public async Task JoinShoppingList(string shoppingListId)
    {
        Console.WriteLine(shoppingListId);
        await Groups.AddToGroupAsync(Context.ConnectionId, shoppingListId);
        await Clients.Caller.SendAsync("JoinShoppingList", shoppingListId);
    }

    //public async Task CreateShoppingList()
    //{
    //    string shoppingListId = Guid.NewGuid().ToString();

    //    _groupIdShoppingLists.TryAdd(shoppingListId, []);
    //    await Groups.AddToGroupAsync(Context.ConnectionId, shoppingListId);
    //    await Clients.Caller.SendAsync("ShoppingListCreated", shoppingListId);
    //}

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _counter--;
        Console.WriteLine($"A client disconnected {DateTime.Now}. Total clients: {_counter}");
        await base.OnDisconnectedAsync(exception);
    }

    public static class GeneratorHubGroupName
    {
        public static string GenerateGroupName(int chatId) => "group" + chatId;
        public static string GenerateContactName(int chatId) => "contact" + chatId;
    }
}


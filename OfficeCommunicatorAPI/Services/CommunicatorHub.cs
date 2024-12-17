using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OfficeCommunicatorAPI.DTO;
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
    private readonly IMapper _mapper;
    private static int _counter;

    public CommunicatorHub(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper)
    {
        _mapper = mapper;
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

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateUserGroupName(userId));

        foreach (var group in user.Groups) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(group.ChatId));
        foreach (var contact in user.Contacts) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(contact.ChatId));
        _counter++;
        Console.WriteLine($"A client connected {DateTime.Now}. Total clients: {_counter}");
        Console.WriteLine($"User was added to {user.Groups.Count()} groups");
        await base.OnConnectedAsync();
    }



    public async Task<Group?> JoinGroup(int groupId)
    {
        if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        Group? group = await _groupRepository.GetGroupWithUsers(groupId);
        if (group == null || group.Users.Any(u => u.Id == userId)) return null;
        if (await _groupRepository.AddUserToGroupAsync(groupId, userId)) return null;

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(group.ChatId));

        group.Users = new List<User>();
        return group;
    }



    public async Task<Contact?> CreateContact(int associatedUserId)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return null;

        if(userId == associatedUserId) return null;

        Contact? contact = await _contactRepository.GetByUserIdAndAssociatedUserIdAsync(userId, associatedUserId);
        if (contact != null) return null;

        Contact[]? contacts = await _contactRepository.AddContactAsync(associatedUserId, userId);

        if (contacts == null) return null;

        contact = contacts[0];
        Contact associatedContact = contacts[1];

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(contact.ChatId));
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(associatedUserId)).SendAsync("ReceiveContact", associatedContact);

        return contact;
    }

    public async Task SubscribeContact(int contactId)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return;
        User? user = await _userRepository.GetByIdAsync(userId);
        if (user == null) return;

        Contact? contact = await _contactRepository.GetByIdAsync(contactId);
        if (contact == null) return;

        if(contact.UserId != userId && contact.AssociatedUserId != userId) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateGroupName(contact.ChatId));
    }


    public async Task SendMessage(Message message)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId) || userId != message.UserId) return;
        if ((!await _groupRepository.IsUserGroup(message.ChatId, userId)) && (!await _contactRepository.IsUserContact(message.ChatId, userId))) return;

        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateGroupName(message.ChatId)).SendAsync("ReceiveMessage", message);
    }

    //public async Task<MessageSignalRModel?> SendMessageGroup(int chatId, int messageIndex, string content)
    //{
    //    if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return null;

    //    Message message = new Message
    //    {
    //        ChatId = chatId,
    //        UserId = userId,
    //        Content = content,
    //        Date = DateTime.Now
    //    };
    //    bool result = await _messageRepository.AddMessageAsync(message, async () => await _groupRepository.IsUserGroup(chatId, userId));
    //    if (!result) return null;

    //    await Clients.OthersInGroup(GeneratorHubGroupName.GenerateGroupName(message.ChatId)).SendAsync("ReceiveGroupMessage", message);
    //    MessageSignalRModel messageResult = _mapper.Map<MessageSignalRModel>(message);
    //    messageResult.SqliteIndex = messageIndex;
    //    return messageResult;
    //}


    //public async Task<MessageSignalRModel?> SendMessageContact(int chatId, int messageIndex, string content)
    //{
    //    if(!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return null;

    //    Message message = new Message
    //    {
    //        ChatId = chatId,
    //        UserId = userId,
    //        Content = content,
    //        Date = DateTime.Now
    //    };
    //    bool result = await _messageRepository.AddMessageAsync(message, async () => await _contactRepository.IsUserContact(userId, chatId));
    //    if (!result) return null;

    //    await Clients.OthersInGroup(GeneratorHubGroupName.GenerateContactName(message.ChatId)).SendAsync("ReceiveContactMessage", message);
    //    MessageSignalRModel messageResult = _mapper.Map<MessageSignalRModel>(message);
    //    messageResult.SqliteIndex = messageIndex;
    //    return messageResult;
    //}


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
            hubGroupName = GeneratorHubGroupName.GenerateGroupName(contact.ChatId);
        }else throw new ArgumentException("Invalid communication type");


        bool result = await _messageRepository.RemoveAsync(messageId, userId);
        if (!result) throw new Exception("Message not removed");
        
        await Clients.OthersInGroup(hubGroupName).SendAsync("RemoveMessage", message);
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _counter--;
        Console.WriteLine($"A client disconnected {DateTime.Now}. Total clients: {_counter}");
        await base.OnDisconnectedAsync(exception);
    }


    public static class GeneratorHubGroupName
    {
        public static string GenerateGroupName(int chatId) => "group" + chatId;
        //public static string GenerateGroupName(int chatId) => "group" + chatId;
        //public static string GenerateContactName(int chatId) => "contact" + chatId;
        public static string GenerateUserGroupName(int userId) => "user" + userId;
    }
}


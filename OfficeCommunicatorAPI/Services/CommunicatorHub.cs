using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OfficeCommunicatorAPI.DTO;
using OfficeCommunicatorAPI.Models;
using OfficeCommunicatorAPI.Repositories;
using OfficeCommunicatorAPI.Services.Checkers;

namespace OfficeCommunicatorAPI.Services;

[Authorize]
public class CommunicatorHub : Hub
{
    private readonly UserRepository _userRepository;
    private readonly GroupRepository _groupRepository;
    private readonly ContactRepository _contactRepository;
    private readonly MessageRepository _messageRepository;
    private readonly GroupChecker _groupChecker;
    private readonly ContactChecker _contactChecker;
    private readonly IMapper _mapper;
    private static int _counter;

    public CommunicatorHub(IMapper mapper, OfficeDbContext dbContext, AuthHelper authHelper)
    {
        _mapper = mapper;
        _messageRepository = new MessageRepository(dbContext, mapper);
        _userRepository = new UserRepository(dbContext, mapper, authHelper);
        _groupRepository = new GroupRepository(dbContext, mapper);
        _contactRepository = new ContactRepository(dbContext, mapper);
        _groupChecker = new GroupChecker(dbContext);
        _contactChecker = new ContactChecker(dbContext);
    }



    public override async Task OnConnectedAsync()
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");

        User? user = await _userRepository.GetByIdWithIncludeAsync(userId);
        if (user == null) throw new ArgumentException("User not found");

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateUserGroupName(userId));

        foreach (var group in user.Groups) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(group.ChatId));
        foreach (var contact in user.Contacts) await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(contact.ChatId));
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

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(group.ChatId));

        group.Users = new List<User>();
        return group;
    }



    public async Task<Contact?> CreateContact(int associatedUserId)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return null;

        if(userId == associatedUserId) return null;

        Contact? contact = await _contactRepository.GetByUserIdAndAssociatedUserIdAsync(userId, associatedUserId);
        if (contact != null) return null;

        (contact, Contact? associatedContact) = await _contactRepository.AddContactAsync(associatedUserId, userId);

        if (contact == null) return null;

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(contact.ChatId));
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

        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(contact.ChatId));
    }


    public async Task SendMessage(Message message)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId) || userId != message.UserId) return;
        if ((!await _groupChecker.CheckPermissionUser(userId, message.ChatId)) && (!await _contactChecker.CheckPermissionUser(userId, message.ChatId))) return;

        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(message.ChatId)).SendAsync("ReceiveMessage", message);
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
            hubGroupName = GeneratorHubGroupName.GenerateChatName(group.ChatId);
        }else if (communication == "contact")
        {
            Contact? contact = await _contactRepository.GetByIdAsync(communicationId);
            if (contact == null) throw new ArgumentException("Contact not found");
            hubGroupName = GeneratorHubGroupName.GenerateChatName(contact.ChatId);
        }else throw new ArgumentException("Invalid communication type");


        bool result = await _messageRepository.RemoveAsync(messageId, userId);
        if (!result) throw new Exception("Message not removed");
        
        await Clients.OthersInGroup(hubGroupName).SendAsync("RemoveMessage", message);
    }


    public async Task CallUser(int userId, int callerUserId)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(callerUserId)).SendAsync("ReceiveUserCall", userId);
    }

    public async Task CallGroup(int chatId, string zoomUrl)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateChatName(chatId)).SendAsync("ReceiveGroupCall", chatId, zoomUrl);
    }

    public async Task AnswerCall(int userId, string identity)
    {
        Console.WriteLine($"Identity is {identity}");
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(userId)).SendAsync("ReceiveAnswer", identity);
    }


    public async Task RejectCall(int rejecterUserId, int callerUserId)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(callerUserId)).SendAsync("RejectAnswer", rejecterUserId);
    }


    public async Task CancelCall(int cancelUserId, int callerUserId)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(callerUserId)).SendAsync("OnCancelCall", cancelUserId);
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _counter--;
        Console.WriteLine($"A client disconnected {DateTime.Now}. Total clients: {_counter}");
        await base.OnDisconnectedAsync(exception);
    }



    public static class GeneratorHubGroupName
    {
        public static string GenerateChatName(int chatId) => "chat" + chatId;
        public static string GenerateUserGroupName(int userId) => "user" + userId;
    }
}


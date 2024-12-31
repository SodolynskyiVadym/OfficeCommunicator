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

        Console.WriteLine($"USer {user.Name} was subscribed");
        foreach (var group in user.Groups)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(group.ChatId));
            Console.WriteLine($"User was added to group {GeneratorHubGroupName.GenerateChatName(group.ChatId)}");
        }
        foreach (var contact in user.Contacts)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(contact.ChatId));
            Console.WriteLine($"User was added to group {GeneratorHubGroupName.GenerateChatName(contact.ChatId)}");

        }
        _counter++;


        Console.WriteLine($"A client connected {DateTime.Now}. Total clients: {_counter}");
        Console.WriteLine($"User was added to {user.Groups.Count() + user.Contacts.Count()} groups");
        await base.OnConnectedAsync();
    }



    public async Task SubscribeChat(int chatId)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) return;
        await Groups.AddToGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(chatId));
    }


    public async Task UnsubscribeChat(int chatId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GeneratorHubGroupName.GenerateChatName(chatId));
    }


    public async Task AddUserToChat(User user, Group group)
    {
        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(group.ChatId)).SendAsync("OnAddUserToChat", user);
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(user.Id)).SendAsync("OnAddGroup", group);
    }

    public async Task CreateContact(int userId, Contact contact)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(userId)).SendAsync("OnAddContact", contact);
    }


    public async Task RemoveUserFromGroup(int userId, int chatId)
    {
        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(chatId)).SendAsync("OnRemoveUserFromGroup", userId, chatId);
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(userId)).SendAsync("OnRemoveChat", chatId);
    }

    public async Task RemoveContact(int userId, int chatId)
    {
        await Clients.Group(GeneratorHubGroupName.GenerateUserGroupName(userId)).SendAsync("OnRemoveChat", chatId);
    }


    public async Task SendMessage(Message message)
    {
        Console.WriteLine($"Sending message {message.Content} with {message.Documents.Count} documents");
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId) || userId != message.UserId) return;
        if ((!await _groupChecker.CheckPermissionUser(userId, message.ChatId)) && (!await _contactChecker.CheckPermissionUser(userId, message.ChatId))) return;

        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(message.ChatId)).SendAsync("ReceiveMessage", message);
    }



    public async Task UpdateMessage(Message message)
    {
        if (!int.TryParse(Context.User?.FindFirst("userId")?.Value, out var userId)) throw new ArgumentException("Invalid user id");
        if (userId != message.UserId) throw new ArgumentException("You aren't the author of the message");

        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(message.ChatId)).SendAsync("OnUpdateMessage", message);
    }


    public async Task RemoveMessage(int chatId, int messageId)
    {
        Console.WriteLine($"Delete message int chat id {chatId}, message Id {messageId}");
        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(chatId)).SendAsync("OnRemoveMessage", chatId, messageId);
    }


    public async Task DeleteDocument(int documentId, int messageId, int chatId)
    {
        Console.WriteLine($"Delete document {documentId}, messageid {messageId}, chatId {chatId} in group {GeneratorHubGroupName.GenerateChatName(chatId)}");
        await Clients.OthersInGroup(GeneratorHubGroupName.GenerateChatName(chatId)).SendAsync("OnRemoveDocument", documentId, messageId, chatId);
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


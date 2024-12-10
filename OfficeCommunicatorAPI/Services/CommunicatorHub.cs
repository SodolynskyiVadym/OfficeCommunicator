using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;

namespace OfficeCommunicatorAPI.Services;

public class CommunicatorHub : Hub
{
    private static readonly ConcurrentDictionary<string, List<string>> _groupIdShoppingLists = new();
    private static int counter = 0;

    public override async Task OnConnectedAsync()
    {
        counter++;
        Console.WriteLine($"A client connected {DateTime.Now}. Total clients: {counter}");
        await base.OnConnectedAsync();
    }


    public async Task SendMessage(string chatId, string message, string user)
    {

    }

    public async Task RemoveMessage(string chatId, string messageId)
    {

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

    //public async Task JoinShoppingList(string shoppingListId)
    //{
    //    if (_groupIdShoppingLists.TryGetValue(shoppingListId, out var list))
    //    {
    //        await Groups.AddToGroupAsync(Context.ConnectionId, shoppingListId);
    //        await Clients.Caller.SendAsync("JoinShoppingList", shoppingListId, list);
    //    }
    //}

    //public async Task CreateShoppingList()
    //{
    //    string shoppingListId = Guid.NewGuid().ToString();

    //    _groupIdShoppingLists.TryAdd(shoppingListId, []);
    //    await Groups.AddToGroupAsync(Context.ConnectionId, shoppingListId);
    //    await Clients.Caller.SendAsync("ShoppingListCreated", shoppingListId);
    //}

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        counter--;
        Console.WriteLine($"A client disconnected {DateTime.Now}. Total clients: {counter}");
        await base.OnDisconnectedAsync(exception);
    }
}
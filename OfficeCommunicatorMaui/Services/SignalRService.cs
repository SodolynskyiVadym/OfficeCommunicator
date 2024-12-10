using Microsoft.AspNetCore.SignalR.Client;

namespace OfficeCommunicatorMAUI.Services
{
    public class SignalRService
    {
        private string ServerUrl { get; set; }

        public SignalRService(string serverUrl)
        {
            ServerUrl = serverUrl;

        }

        public HubConnection InitializeConnectionAsync()
        {
            HubConnection hubConnection = new HubConnectionBuilder()
                .WithUrl(ServerUrl)
                .Build();
            return hubConnection;
        }

        public async Task JoinList(string listId, HubConnection hubConnection)
        {
            await hubConnection.InvokeAsync("JoinShoppingList", listId);
        }

        public async Task CreateList(HubConnection hubConnection)
        {
            await hubConnection.InvokeAsync("CreateShoppingList");
        }

        public async Task AddItem(string listId, string item, HubConnection hubConnection)
        {
            await hubConnection.InvokeAsync("AddItem", listId, item);
        }

        public async Task RemoveItem(string listId, string item, HubConnection hubConnection)
        {
            await hubConnection.InvokeAsync("RemoveItem", listId, item);
        }

        public async Task DisconnectAsync(HubConnection hubConnection)
        {
            await hubConnection.StopAsync();
            await hubConnection.DisposeAsync();
        }
    }
}

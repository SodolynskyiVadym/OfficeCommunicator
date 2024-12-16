using System.Collections.Concurrent;
using System.Text.Json;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services
{
    public static class SecureStorageService
    {
        private const string TokenKey = "JwtToken";
        private const string MessagesKey = "Messages";
        private const string CounterKey = "Counter";

        public static async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        public static async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey);
        }

        public static void RemoveToken()
        {
            SecureStorage.Remove(TokenKey);
        }

        public static async Task SaveMessagesAsync(ConcurrentDictionary<int, MessageSecretStorageModel> messages, int counter)
        {
            string json = JsonSerializer.Serialize(messages);
            await SecureStorage.SetAsync(MessagesKey, json);
            await SecureStorage.SetAsync(CounterKey, counter.ToString());
        }

        public static async Task<ConcurrentDictionary<int, MessageSecretStorageModel>> GetMessagesAsync()
        {
            string? json = await SecureStorage.GetAsync(MessagesKey);
            if (json == null)
            {
                return new ConcurrentDictionary<int, MessageSecretStorageModel>();
            }
            ConcurrentDictionary<int, MessageSecretStorageModel>? messages = JsonSerializer.Deserialize<ConcurrentDictionary<int, MessageSecretStorageModel>>(json);
            return messages ?? new ConcurrentDictionary<int, MessageSecretStorageModel>();
        }

        public static async Task<int> GetCounterAsync()
        {
            string? counter = await SecureStorage.GetAsync(CounterKey);
            return counter == null ? 0 : int.Parse(counter);
        }
    }
}
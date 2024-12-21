using System.Collections.Concurrent;
using System.Text.Json;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services
{
    public class SecureStorageService
    {
        private int random { get; set; }

        public SecureStorageService(int random)
        {
            this.random = random;
        }

        private const string TokenKey = "JwtToken";
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM0NzgxNTg0LCJleHAiOjE3MzQ4Njc5ODQsImlhdCI6MTczNDc4MTU4NH0.Nr8rmKSsoCSNb0JtS0R9ZTt5kyvGHaToAEV4yI7UTYKq_gAP55usr7pooWFkB8b8txTPxDBHWkMWLBXr4I9zbg";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzQ3ODE1NjAsImV4cCI6MTczNDg2Nzk2MCwiaWF0IjoxNzM0NzgxNTYwfQ.lM8Gs2dMWzQizFvXLhWa04o42AexWOLaHJw_vuYS5trvliBm5y4UxSswC3cLv6dGgNMQJPcEEkrSm07R4SVRMQ";
        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        //public async Task<string?> GetTokenAsync()
        //{
        //    return await SecureStorage.GetAsync(TokenKey);
        //}

        public async Task<string?> GetTokenAsync()
        {
            return random switch
            {
                1 => jack,
                2 => john,
                _ => null
            };
        }



        public void RemoveToken()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}
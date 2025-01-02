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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiZW1haWwiOiJqYWNrQGdtYWlsLmNvbSIsIm5pY2tuYW1lIjoiamFjayIsIm5iZiI6MTczNTczNDcxNCwiZXhwIjoxNzM1ODIxMTE0LCJpYXQiOjE3MzU3MzQ3MTR9._wK7NAgmcVBFEAblGzGKY_f73zKlTIrCgKOYueepyHNCBIPp6eVnOWbLBd6J07woNi7nrtgsAtpIMglBALbdqQ";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwiZW1haWwiOiJqb2huQGdtYWlsLmNvbSIsIm5pY2tuYW1lIjoiam9obiIsIm5iZiI6MTczNTczNDc0OCwiZXhwIjoxNzM1ODIxMTQ4LCJpYXQiOjE3MzU3MzQ3NDh9.023T_UhHPBYDXFG7lYJ5xcs-hCc4-wn42h8zMhvhfv3YaXMcGaSTyhF3V8yE7O0JVkstvgrTDzCeKyUvelVjIw";
        
        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TokenKey, token);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await SecureStorage.GetAsync(TokenKey);
        }

        //public async Task<string?> GetTokenAsync()
        //{
        //    return random switch
        //    {
        //        1 => jack,
        //        2 => john,
        //        _ => null
        //    };

        //    //return john;
        //}



        public void RemoveToken()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}
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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM0Njk0MjMwLCJleHAiOjE3MzQ3ODA2MzAsImlhdCI6MTczNDY5NDIzMH0.euwwrRyIW1CLJ3Ir1d85riOicyO23Vku904NIYzXPfPJOBM6mUeMuu4J-AyMFYmGCY_o1EoWJ3eZjWFuZU4mBQ";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzQ2OTQxOTEsImV4cCI6MTczNDc4MDU5MSwiaWF0IjoxNzM0Njk0MTkxfQ.uQYz1wp1Hzb44PGFYKhPytJ5-6Yf9qGvQLsknf3LKjRx9gKawNckIyJNr2pWhVrxCzi4SG0c2tc52YRkh9VJyg";
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
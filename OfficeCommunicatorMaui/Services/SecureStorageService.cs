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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIyIiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1MDg0ODY3LCJleHAiOjE3MzUxNzEyNjcsImlhdCI6MTczNTA4NDg2N30.98pWvvoBeqzgToTG3bLIcXfRxV77sPRwZMYD0672fpSoe6Zya1KtgW0KQdVd3570Y58nmaEMEGG_BhhbpH8T5w";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiIxIiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzUwODQ4MzcsImV4cCI6MTczNTE3MTIzNywiaWF0IjoxNzM1MDg0ODM3fQ.UQBOBJbhkwrw8LL3FfLe4hbuf-S27SO79fiXogxQV6EQRYCmQXtpoCsdcKZHD12UfQS0KpSur8U55nsGDFC2CQ";
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
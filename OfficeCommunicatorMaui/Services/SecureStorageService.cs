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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3IiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1MjM0NjUzLCJleHAiOjE3MzUzMjEwNTMsImlhdCI6MTczNTIzNDY1M30.FnStXIn59vFznhBfrwAiLcFIIGzNdxBWNpQGLgIPzlXlSBeLdV32o27XLR8ZNolIWApYky28bQAxk5fw_1YsRw";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzUyMzQ2OTMsImV4cCI6MTczNTMyMTA5MywiaWF0IjoxNzM1MjM0NjkzfQ.U8G4noYIrERfjvpD-XJ3tMFnM5zor8COkcsxKXMlFhbH0PjGO3nRujFXVfY7fq0Kt1lPBSXRtHXxNUfd-Z47Tw";
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
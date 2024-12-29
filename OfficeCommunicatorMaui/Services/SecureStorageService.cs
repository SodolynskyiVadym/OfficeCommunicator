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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3IiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1NDI1MTE2LCJleHAiOjE3MzU1MTE1MTYsImlhdCI6MTczNTQyNTExNn0.-1sxnp9le7fm2fuBdp2Ev3liD0HqIhrGEM55u2bD8OYOXzKKK5zie85CPeUGQH6D8qgnwLx-3PYvsNT8jh0DMg";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzU0MjUwOTMsImV4cCI6MTczNTUxMTQ5MywiaWF0IjoxNzM1NDI1MDkzfQ.gB6aZE-V3s3ve9FOFVKDno0wblvTsAhErHMCDSYkeRy3Uv069PPaUp66PzYoeUbCMWtMcnCUi2er4nuySpJr6w";
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

            //return john;
        }



        public void RemoveToken()
        {
            SecureStorage.Remove(TokenKey);
        }
    }
}
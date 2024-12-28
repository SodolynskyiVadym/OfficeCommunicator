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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3IiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1MzI0ODAyLCJleHAiOjE3MzU0MTEyMDIsImlhdCI6MTczNTMyNDgwMn0.jWLIx7M1YWXR9K_3OV9oYRZCQd25xR6w3GecDx6zipLzuJmDO6esPkt9FW79vjQQGth9kOkWu2Y2x1Jkv2JfSA";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzUzMjQ3NzksImV4cCI6MTczNTQxMTE3OSwiaWF0IjoxNzM1MzI0Nzc5fQ.lEC-11Fk9OTY_C9dBdO84mNqtHL3AZgS2IUwUVeUww4M0IKCp_SNHI2ff_oWnLsudmOds_Z4Q4SUG08q9iivrg";
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
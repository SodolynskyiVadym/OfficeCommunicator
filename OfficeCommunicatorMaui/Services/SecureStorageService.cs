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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3IiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1NTExOTA3LCJleHAiOjE3MzU1OTgzMDcsImlhdCI6MTczNTUxMTkwN30.H3fwIxr-qL0qGy_RTH5_XuKPRetTFl8luzKOHltPm-QG2AU94xw8qAh6BhXcr-r2BwP1vvHJ7fG1OGEZGx_9xg";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzU1MTE4ODgsImV4cCI6MTczNTU5ODI4OCwiaWF0IjoxNzM1NTExODg4fQ.H9qKhEpApC3tn4lPxLishtza2HboRXuRW_Mc5IYsmHLgm0kHnAyssdApecXVIVM_JG8EjMvSOUkSVp-L-SxAVA";
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
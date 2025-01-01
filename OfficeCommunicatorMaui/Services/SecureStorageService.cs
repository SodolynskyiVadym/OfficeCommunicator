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
        private const string jack = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI3IiwiZW1haWwiOiJqYWNrQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKYWNrIiwibmJmIjoxNzM1NjAwMDA2LCJleHAiOjE3MzU2ODY0MDYsImlhdCI6MTczNTYwMDAwNn0.L4mnZy93KHdjjMx_O-A9JV9V_Qq39kT2-KmeNtPXDnkFiecfoV9u0TB7Ic_TXkIJiSyaVrztl3sjNPd7ZQ6xhg";
        private const string john = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJ1c2VySWQiOiI1IiwiZW1haWwiOiJqb2huZG9lQGV4YW1wbGUuY29tIiwibmlja25hbWUiOiJKb2hubnkiLCJuYmYiOjE3MzU1OTk5ODksImV4cCI6MTczNTY4NjM4OSwiaWF0IjoxNzM1NTk5OTg5fQ.sLlQJdoYWUFyrwLBgsA2lo_R78GhsATzUvWkzS1OWtD7uA_jo4AHirUn0Lqz6paIbqT9mMGC0locGIHSSDbgFQ";
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
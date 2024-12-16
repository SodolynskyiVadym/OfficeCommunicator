﻿using System.Collections.Concurrent;
using System.Text.Json;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services
{
    public static class SecureStorageService
    {
        private const string TokenKey = "JwtToken";

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
    }
}
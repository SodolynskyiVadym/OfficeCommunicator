﻿using System.Net.Http.Headers;
using System.Net.Http.Json;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services.API
{
    public class AuthApiService
    {
        private HttpClient _httpClient;
        private string _url;

        public AuthApiService(string url, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = url + "/user";
        }

        public async Task<ServerResponse<User>> GetUserAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_url + "/get");
                return new ServerResponse<User>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<User>(null, 500, false, e.Message);
            }
        }


        public async Task<List<User>?> GetUsersAsync()
        {
            var response = await _httpClient.GetAsync(_url + "/getAll");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<User>>();
            }
            else
            {
                throw new Exception("Failed to get users");
            }
        }

        public async Task<string?> LoginAsync(string username, string password)
        {
            var loginRequest = new { Email = username, Password = password };
            var response = await _httpClient.PostAsync(_url + "/login", JsonRequestConvert.ConvertToJsonRequest(loginRequest));
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed: {error}");
            }
        }

        public async Task<string?> SignUp(string userName, string email, string uniqueName, string zoomUrl, string password)
        {
            var signUpRequest = new { Email = email, Name = userName, UniqueName = uniqueName, ZoomUrl = zoomUrl, Password = password };
            var response = await _httpClient.PostAsync(_url + "/signup", JsonRequestConvert.ConvertToJsonRequest(signUpRequest));

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Registration failed: {error}");
            }
        }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }
}

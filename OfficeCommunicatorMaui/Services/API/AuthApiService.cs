using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Services.API
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string url = "http://localhost:5207/user";

        public async Task<string?> LoginAsync(string username, string password)
        {
            var loginRequest = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync(url + "/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return result?.Token;
            }
            else
            {
                // Handle error response
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Login failed: {error}");
            }
        }

        public async Task RegisterAsync(string username, string password)
        {
            var registerRequest = new { Username = username, Password = password };
            var response = await _httpClient.PostAsJsonAsync("https://yourapi.com/api/auth/register", registerRequest);

            if (!response.IsSuccessStatusCode)
            {
                // Handle error response
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

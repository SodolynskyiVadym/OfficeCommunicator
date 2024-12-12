using System.Net.Http.Json;

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

        public async Task<string?> LoginAsync(string username, string password)
        {
            var loginRequest = new { Email = username, Password = password };
            //var response = await _httpClient.PostAsync("http://localhost:5207/user/login", JsonRequestConvert.ConvertToJsonRequest(loginRequest));
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

        public async Task<string?> RegisterAsync(string username, string password)
        {
            var registerRequest = new { Email = username, Password = password };
            var response = await _httpClient.PostAsync(_url + "/signup", JsonRequestConvert.ConvertToJsonRequest(registerRequest));

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

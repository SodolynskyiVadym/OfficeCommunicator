using System.Net.Http.Headers;
using System.Net.Http.Json;
using OfficeCommunicatorMaui.DTO;
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


        public async Task<ServerResponse<List<User>>> GetUsersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync(_url + "/getAll");
                return new ServerResponse<List<User>>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<List<User>>(null, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<LoginResponse>> LoginAsync(string username, string password)
        {
            var loginRequest = new { Email = username, Password = password };
            try
            {
                var response = await _httpClient.PostAsync(_url + "/login", JsonRequestConvert.ConvertToJsonRequest(loginRequest));
                return new ServerResponse<LoginResponse>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<LoginResponse>(null, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<LoginResponse>> SignUp(string userName, string email, string uniqueName, string zoomUrl, string password)
        {
            var signUpRequest = new { Email = email, Name = userName, UniqueName = uniqueName, ZoomUrl = zoomUrl, Password = password };
            try
            {
                var response = await _httpClient.PostAsync(_url + "/signup", JsonRequestConvert.ConvertToJsonRequest(signUpRequest));
                return new ServerResponse<LoginResponse>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<LoginResponse>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<bool>> UpdateUserAsync(UserUpdateDto userUpdate, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.PostAsync(_url + "/update", JsonRequestConvert.ConvertToJsonRequest(userUpdate));
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }

        public class LoginResponse
        {
            public string Token { get; set; }
        }
    }
}

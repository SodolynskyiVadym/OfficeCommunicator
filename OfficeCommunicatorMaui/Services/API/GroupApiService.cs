using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using OfficeCommunicatorMaui.DTO;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services.API
{
    public class GroupApiService
    {
        private HttpClient _httpClient;
        private string _url;

        public GroupApiService(string url, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = url + "/chat";
        }

        public async Task<Group?> CreateGroupAsync(GroupCreateDto group, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync(_url + "/create-group", JsonRequestConvert.ConvertToJsonRequest(group));
            if (response.IsSuccessStatusCode)
            {
                Group? result = await response.Content.ReadFromJsonAsync<Group>();
                return result;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to create group: {error}");
            }
        }


        public async Task<User?> AddUserToChatAsync(int userId, int groupId, string token)
        {
            var body = new { UserId = userId, GroupId = groupId };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync(_url + $"/add-user-to-group", JsonRequestConvert.ConvertToJsonRequest(body));
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<User>();
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to add user to chat: {error}");
            }
        }


        public async Task<bool> AddAdminAsync(int userId, int groupId, string token)
        {
            var body = new { UserId = userId, GroupId = groupId };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsync(_url + $"/add-admin", JsonRequestConvert.ConvertToJsonRequest(body));
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to add admin: {error}");
            }
        }


        public async Task<bool> LeaveGroup(int groupId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync(_url + $"/leave-group/{groupId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to leave group: {error}");
            }
        }

        public async Task<bool> RemoveUserFromGroup(int groupId, int userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync(_url + $"/remove-user-from-group/{groupId}/{userId}");
            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to leave group: {error}");
            }
        }
    }
}

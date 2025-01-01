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

        public async Task<ServerResponse<Group>> CreateGroupAsync(GroupCreateDto group, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.PostAsync(_url + "/create-group", JsonRequestConvert.ConvertToJsonRequest(group));
                return new ServerResponse<Group>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Group>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<User>> AddUserToChatAsync(int userId, int groupId, string token)
        {
            var body = new { UserId = userId, GroupId = groupId };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.PostAsync(_url + $"/add-user-to-group", JsonRequestConvert.ConvertToJsonRequest(body));
                return new ServerResponse<User>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<User>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<bool>> AddAdminAsync(int userId, int groupId, string token)
        {
            var body = new { UserId = userId, GroupId = groupId };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.PostAsync(_url + $"/add-admin", JsonRequestConvert.ConvertToJsonRequest(body));
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<bool>> LeaveGroup(int groupId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.DeleteAsync(_url + $"/leave-group/{groupId}");
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<bool>> RemoveUserFromGroup(int groupId, int userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.DeleteAsync(_url + $"/remove-user-from-group/{groupId}/{userId}");
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }
    }
}

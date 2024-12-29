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
    }
}

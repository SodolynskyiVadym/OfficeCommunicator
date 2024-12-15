using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using OfficeCommunicatorMaui.Models;
using Contact = OfficeCommunicatorMaui.Models.Contact;

namespace OfficeCommunicatorMaui.Services.API
{
    public class ChatApiService
    {
        private HttpClient _httpClient;
        private string _url;

        public ChatApiService(string url, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = url + "/chat";
        }

        public async Task<Group?> GetGroupAsync(int groupId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(_url + $"/get-group/{groupId}");
            if (response.IsSuccessStatusCode)
            {
                Group? result = await response.Content.ReadFromJsonAsync<Group>();
                return result;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to get group: {error}");
            }
        }

        public async Task<Contact?> GetContactAsync(int contactId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(_url + $"/get-contact/{contactId}");
            if (response.IsSuccessStatusCode)
            {
                Contact? result = await response.Content.ReadFromJsonAsync<Contact>();
                return result;
            }
            else
            {
                var error = response.ReasonPhrase;
                throw new Exception($"Failed to get contact: {error}");
            }
        }
    }
}

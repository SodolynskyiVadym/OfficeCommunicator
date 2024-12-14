using System.Net.Http.Headers;
using System.Net.Http.Json;
using OfficeCommunicatorMaui.Models;

namespace OfficeCommunicatorMaui.Services.API
{
    public class ContactApiService
    {
        private HttpClient _httpClient;
        private string _url;

        public ContactApiService(string url, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = url + "/chat";
        }

        public async Task<Models.Contact?> GetContactAsync(int contactId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(_url + $"/get-contact/{contactId}");
            if (response.IsSuccessStatusCode)
            {
                Models.Contact? result = await response.Content.ReadFromJsonAsync<Models.Contact>();
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

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

        public async Task<ServerResponse<Models.Contact>> GetContactAsync(int contactId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_url + $"/get-contact/{contactId}");
                return new ServerResponse<Models.Contact>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Models.Contact>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<Models.Contact>> CreateContactAsync(int userId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var body = new { AssociatedUserId = userId };
            try
            {
                var response = await _httpClient.PostAsync(_url + "/create-contact", JsonRequestConvert.ConvertToJsonRequest(body));
                return new ServerResponse<Models.Contact>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Models.Contact>(null, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<bool>> RemoveContact(int chatId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.DeleteAsync(_url + $"/delete-contact/{chatId}");
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }
    }
}

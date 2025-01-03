using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
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


        public async Task<ServerResponse<Group>> GetGroupAsync(int groupId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_url + $"/get-group/{groupId}");
                return new ServerResponse<Group>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Group>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<Contact>> GetContactAsync(int associatedUserId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_url + $"/get-contact/{associatedUserId}");
                return new ServerResponse<Contact>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Contact>(null, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<Message>> CreateMessageAsync(MessageStorageModel message, List<IBrowserFile> files, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                using var content = new MultipartFormDataContent
                {
                    { JsonRequestConvert.ConvertToJsonRequest(message), "messageDtoJson" }
                };

                foreach (var file in files)
                {
                    var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(fileContent, "files", file.Name);
                }

                var response = await _httpClient.PostAsync(_url + "/create-message", content);
                return new ServerResponse<Message>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Message>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<Message>> UpdateMessageAsync(MessageStorageModel message, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.PostAsync(_url + "/update-message", JsonRequestConvert.ConvertToJsonRequest(message));
                return new ServerResponse<Message>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<Message>(null, 500, false, e.Message);
            }
        }


        public async Task<ServerResponse<bool>> DeleteMessageAsync(int messageId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.DeleteAsync(_url + $"/delete-message/{messageId}");
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }

        public async Task<ServerResponse<bool>> DeleteDocumentAsync(int documentId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.DeleteAsync(_url + $"/delete-document/{documentId}");
                return new ServerResponse<bool>(response);
            }
            catch (Exception e)
            {
                return new ServerResponse<bool>(false, 500, false, e.Message);
            }
        }



        public async Task<ServerResponse<DownloadFileResponseDto>> DownLoadFileAsync(string fileName, int messageId, int documentId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_url + $"/download/{messageId}/{documentId}");
                if (response.IsSuccessStatusCode)
                {
                    var fileContent = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                    var downloadFileResponse = new DownloadFileResponseDto(fileContent, fileName, contentType);
                    return new ServerResponse<DownloadFileResponseDto>(downloadFileResponse, 200, true, null);
                }
                else
                {
                    return new ServerResponse<DownloadFileResponseDto>(null, (int)response.StatusCode, false, response.ReasonPhrase);
                }
            }
            catch (Exception e)
            {
                return new ServerResponse<DownloadFileResponseDto>(null, 500, false, e.Message);
            }
        }
    }
}

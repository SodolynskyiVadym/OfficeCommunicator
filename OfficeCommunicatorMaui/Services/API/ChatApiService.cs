using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using OfficeCommunicatorMaui.DTO;
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


        //public async Task<bool> UploadFile(IBrowserFile file, string token)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    if (file == null) throw new Exception("File is null");

        //    var content = new MultipartFormDataContent();
        //    var stream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024); // Limit to 10 MB
        //    var fileContent = new StreamContent(stream);
        //    content.Add(fileContent, "file", file.Name);

        //    var response = await _httpClient.PostAsync(_url + "/updload-file", content);


        //    if (response.IsSuccessStatusCode)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public async Task<Message?> CreateMessageAsync(MessageStorageDto message, List<IBrowserFile> files, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var content = new MultipartFormDataContent
            {
                { JsonRequestConvert.ConvertToJsonRequest(message), "messageDtoJson" }
            };

            foreach (var file in files)
            {
                Debug.WriteLine($"File to upload: {file.Name}");
                var fileContent = new StreamContent(file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "files", file.Name);
            }

            var response = await _httpClient.PostAsync(_url + "/create-message", content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Message>();
            }
            else
            {
                return null;
            }
        }

        public async Task DownLoadFileAsync(string fileName, int messageId, int documentId, string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync(_url + $"/download/{messageId}/{documentId}");

            //if (response.IsSuccessStatusCode)
            //{
            //    var stream = await response.Content.ReadAsStreamAsync();
            //    var fileName = response.Content.Headers.ContentDisposition?.FileName;
            //    var contentType = response.Content.Headers.ContentType?.MediaType;
            //    await _fileService.SaveFileAsync(stream, fileName, contentType);
            //}

            if (response.IsSuccessStatusCode)
            {
                var fileContent = await response.Content.ReadAsByteArrayAsync();
                var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
                //await JSRuntime.InvokeVoidAsync(
                //    "blazorDownloadFile",
                //    fileName,
                //    contentType,
                //    Convert.ToBase64String(fileContent)
                //);
            }
            else
            {
                Console.WriteLine("Failed to download file");
            }
        }
    }
}

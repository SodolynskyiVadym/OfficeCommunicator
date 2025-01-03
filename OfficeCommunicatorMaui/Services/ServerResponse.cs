using System.Net.Http.Json;

namespace OfficeCommunicatorMaui.Services
{
    public class ServerResponse<T>
    {
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public ServerResponse(HttpResponseMessage response)
        {
            if(Data is not bool) Data = response.Content.ReadFromJsonAsync<T>().Result;
            StatusCode = ((int)response.StatusCode);
            IsSuccess = response.IsSuccessStatusCode;
            ErrorMessage = response.ReasonPhrase;
        }

        public ServerResponse(T? data, int statusCode, bool isSuccess, string? errorMessage)
        {
            Data = data;
            StatusCode = statusCode;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }
    }
}

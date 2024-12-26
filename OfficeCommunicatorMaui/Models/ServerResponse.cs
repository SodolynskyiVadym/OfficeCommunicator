using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OfficeCommunicatorMaui.Models
{
    public class ServerResponse<T>
    {
        public T? Data { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }

        public ServerResponse(HttpResponseMessage response)
        {
            Data = response.Content.ReadFromJsonAsync<T>().Result;
            StatusCode = (int)response.StatusCode;
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

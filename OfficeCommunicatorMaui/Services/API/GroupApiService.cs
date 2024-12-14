namespace OfficeCommunicatorMaui.Services.API
{
    public class GroupApiService
    {
        private HttpClient _httpClient;
        private string _url;

        public GroupApiService(string url, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = url + "/group";
        }

        //public async Task<List<Group>> GetUserGroupsAsync(string token)
        //{
        //    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //    var response = await _httpClient.GetAsync(_url);
        //    if (response.IsSuccessStatusCode)
        //    {
        //        return await response.Content.ReadFromJsonAsync<List<Group>>();
        //    }
        //    else
        //    {
        //        var error = await response.Content.ReadAsStringAsync();
        //        throw new Exception($"Failed to get groups: {error}");
        //    }
        //}
    }
}

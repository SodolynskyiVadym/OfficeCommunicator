namespace OfficeCommunicatorMaui.Services
{
    public static class JsonRequestConvert
    {
        public static StringContent ConvertToJsonRequest(object obj)
        {
            return new StringContent(
                System.Text.Json.JsonSerializer.Serialize(obj),
                System.Text.Encoding.UTF8,
                "application/json"
            );
        }
    }
}

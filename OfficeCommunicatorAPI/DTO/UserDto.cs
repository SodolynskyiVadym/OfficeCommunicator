namespace OfficeCommunicatorAPI.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UniqueName  { get; set; }
        public string Password { get; set; }
        public string ZoomUrl { get; set; }
        public string? AzureToken { get; set; }
        public string? AzureIdentity { get; set; }
    }
}

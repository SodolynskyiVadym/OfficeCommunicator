namespace OfficeCommunicatorAPI.DTO;

public class UserUpdateDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ZoomUrl { get; set; }
    public string? AzureToken { get; set; }
    public string? Password { get; set; }
}
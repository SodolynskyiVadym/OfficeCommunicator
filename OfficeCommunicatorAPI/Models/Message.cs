namespace OfficeCommunicatorAPI.Models;

public class Message
{
    public int Id { get; set; }
    public string? Content { get; set; }
    public DateTime Date { get; set; }
    public byte[]? File { get; set; }
    public byte[]? Image { get; set; }
    public int UserId { get; set; }
}
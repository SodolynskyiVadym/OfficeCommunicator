namespace OfficeCommunicatorAPI.Models;

public class File
{
    public int Id { get; set; }
    public string Name { get; set; }
    public byte[] Data { get; set; }
    public int UserId { get; set; }
    public int MessageId { get; set; }
}
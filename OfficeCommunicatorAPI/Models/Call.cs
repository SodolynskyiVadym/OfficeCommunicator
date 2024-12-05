namespace OfficeCommunicatorAPI.Models;

public class Call
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<int> UserIds { get; set; }
    public IEnumerable<int> MessageIds { get; set; }
    public IEnumerable<int> FileIds { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<Message> Messages { get; set; }
    public IEnumerable<File> Files { get; set; }
}
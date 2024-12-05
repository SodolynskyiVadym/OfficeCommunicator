namespace OfficeCommunicatorAPI.Models;

public class Group
{
    public int Id { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<Message> Messages { get; set; }
}
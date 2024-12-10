using System.Collections;

namespace OfficeCommunicatorAPI.Models;

public class Chat
{
    public int Id { get; set; }
    public IEnumerable<Message> Messages { get; set; }
}
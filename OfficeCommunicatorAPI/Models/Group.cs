using System.Text.Json.Serialization;

namespace OfficeCommunicatorAPI.Models;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UniqueIdentifier { get; set; }
    
    public int ChatId { get; set; }
    public Chat Chat { get; set; }
    public List<User> Admins { get; set; }

    [JsonIgnore]
    public List<User> Users { get; set; }
}
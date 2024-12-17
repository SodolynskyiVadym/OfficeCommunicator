using System.Text.Json.Serialization;

namespace OfficeCommunicatorAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UniqueName  { get; set; }

    [JsonIgnore]
    public byte[] PasswordHash { get; set; }

    [JsonIgnore]
    public byte[] PasswordSalt { get; set; }

    [JsonIgnore]
    public IEnumerable<Group> Groups { get; set; }

    [JsonIgnore]
    public IEnumerable<Contact> Contacts { get; set; }
}
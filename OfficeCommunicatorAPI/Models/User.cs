namespace OfficeCommunicatorAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UniqueName  { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public IEnumerable<Group> Groups { get; set; }
    public IEnumerable<Contact> Contacts { get; set; }
}
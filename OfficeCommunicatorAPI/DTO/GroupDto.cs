using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO;

public class GroupDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<User> Users { get; set; }
    public IEnumerable<User> Admins { get; set; }
}
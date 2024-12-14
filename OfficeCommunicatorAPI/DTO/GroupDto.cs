using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO;

public class GroupDto
{
    public string Name { get; set; }
    public string UniqueIdentifier { get; set; }
    public int UserAuthorId { get; set; }
}
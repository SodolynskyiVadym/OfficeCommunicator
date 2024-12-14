using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO
{
    public class UserPresentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UniqueName { get; set; }
        public IEnumerable<GroupPresentDto> Groups { get; set; }
        public IEnumerable<ContactPresentDto> Contacts { get; set; }
    }
}

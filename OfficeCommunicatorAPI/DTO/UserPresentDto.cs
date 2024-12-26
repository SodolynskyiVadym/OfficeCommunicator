using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO
{
    public class UserPresentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string UniqueName { get; set; }
        public string ZoomUrl { get; set; }
        public string AzureToken { get; set; }
        public string AzureIdentity { get; set; }
        public IEnumerable<GroupPresentDto> Groups { get; set; }
        public IEnumerable<ContactPresentDto> Contacts { get; set; }
    }
}

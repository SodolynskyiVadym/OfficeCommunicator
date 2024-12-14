using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO
{
    public class ContactPresentDto
    {
        public int Id { get; set; }
        public int ChatId { get; set; }
        public int UserId { get; set; }
        public int AssociatedUserId { get; set; }
        public UserDto AssociatedUser { get; set; }
    }
}

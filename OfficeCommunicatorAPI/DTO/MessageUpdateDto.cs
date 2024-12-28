using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO
{
    public class MessageUpdateDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}

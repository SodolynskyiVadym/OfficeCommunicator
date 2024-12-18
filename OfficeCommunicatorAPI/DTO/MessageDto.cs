namespace OfficeCommunicatorAPI.DTO
{
    public class MessageDto
    {
        public string Content { get; set; }
        public string CommunicationType { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}

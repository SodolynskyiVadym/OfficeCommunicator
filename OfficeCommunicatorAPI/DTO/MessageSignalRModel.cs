using OfficeCommunicatorAPI.Models;

namespace OfficeCommunicatorAPI.DTO
{
    public class MessageSignalRModel
    {
        public int Id { get; set; }
        public int DictionaryIndex { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int UserId { get; set; }
        public int ChatId { get; set; }
    }
}
